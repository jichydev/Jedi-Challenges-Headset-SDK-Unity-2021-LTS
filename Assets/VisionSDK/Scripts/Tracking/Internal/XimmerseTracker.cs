using System;
using System.Collections.Generic;
using System.Linq;
using Ximmerse.Vision.Internal;
using UnityEngine;
using Ximmerse.InputSystem;

namespace Ximmerse.Vision.Internal
{
	public class XimmerseTracker : IXimmerseTracker
	{
		#region Public Static Properties

		/// <summary>
		/// The beacon center offset from floor.
		/// Since the camera reports the position to the center of the tracking beaconball, we need to lower the wolrd by the height to the center of the ball from the floor.
		/// </summary>
        public static float BeaconCenterOffsetFromFloor = 0f;//0.0508f;

		#endregion

		#region Public Events

		/// <summary>
		/// Listener for when the beacon gains or loses tracking.
		/// </summary>
		/// <value>The on beacon state change event.</value>
		public EventHandler<BeaconStateChangeEventArgs> OnBeaconStateChange { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Ximmerse.Vision.Internal.XimmerseTracker"/> beacon is tracked.
		/// </summary>
		/// <value><c>true</c> if beacon tracked; otherwise, <c>false</c>.</value>
		public bool IsBeaconTracked { get; private set; }

		/// <summary>
		/// The Hmd.
		/// </summary>
		public HmdPeripheral Hmd { get; private set; }

		#endregion

		#region Private Properties

		// Systems
		private IHeartbeat heartbeat;
		private StereoCamera stereoCamera;
		private IVisionLogger logger;
		private IConnectionManager connectionManager;
		private RotationPrediction rotationPrediction;

		// Smoothing properties
		private Vector3 lastPosition = new Vector3(0, 0, 0);
		private Vector3 tempLastPosition = new Vector3(0, 0, 0);
		private int framesToCatchUp = 0;
		private int lerpFactor;

		// Beacon properties
		private bool sawBeacon = false;
		private Vector3 beaconPosition = Vector3.zero;

		#endregion

		#region Private Constants

		private const int DurationToCatchTracked = 10;
		private const int BeaconBlobID = (int)ColorID.PINK;

		#if UNITY_IOS
		private const int DelayForColorSet = 30;
		#else
		private const int DelayForColorSet = 60;
		#endif

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.Internal.XimmerseTracker"/> class.
		/// </summary>
		/// <param name="logger">The logger instance to use.</param>
		/// <param name="heartbeat">The heartbeat instance to use.</param>
		/// <param name="stereoCamera">The stereo camera instance to use.</param>
		/// <param name = "connectionManager"></param>
		/// <param name = "lerpFactor">The factor by which to multiplly lerp based on distance.</param>
		public XimmerseTracker(IVisionLogger logger, IHeartbeat heartbeat, StereoCamera stereoCamera, IConnectionManager connectionManager, int lerpFactor = 4)
		{
			// Dependent Systems
			this.logger = logger;
			this.heartbeat = heartbeat;
			this.stereoCamera = stereoCamera;
			this.connectionManager = connectionManager;
			this.lerpFactor = lerpFactor;

			StartTracking();

			Hmd = new HmdPeripheral("XHawk-0", "VRDevice");
			connectionManager.AddPeripheral(Hmd);

			// Listeners
			heartbeat.OnBeforeRender += PreRender;
			heartbeat.OnFrameUpdate += Update;

			rotationPrediction = new RotationPrediction();
		}

		#endregion

		#region Unity Methods

		public void Update(object sender, EventArgs arguments)
		{
			Hmd.ControllerInput.UpdateState();
			Hmd.Input.UpdateState();
		}

		public void PreRender(object sender, EventArgs arguments)
		{
			// Have to call this first to update the player based on the camera
			stereoCamera.UpdateRotation();

			// Update the Rotation Prediction
			rotationPrediction.Update(stereoCamera.transform.rotation);

			#region HMD

			if (Hmd.ControllerInput.connectionState == DeviceConnectionState.Connected)
			{
				tempLastPosition = beaconPosition;

				TrackingResult result = XDevicePlugin.GetNodePosition(Hmd.ControllerInput.handle,
																	  0,
																	  BeaconBlobID,
				                                                      ref beaconPosition);

				// Not tracked, but now we are
				if (!IsBeaconTracked && (result & TrackingResult.PositionTracked) != 0)
				{
					IsBeaconTracked = true;

					if (OnBeaconStateChange != null)
					{
						OnBeaconStateChange(this, new BeaconStateChangeEventArgs(true, beaconPosition));
					}
				}
				else if (IsBeaconTracked && (result & TrackingResult.PositionTracked) == 0)
				{
					IsBeaconTracked = false;

					if (OnBeaconStateChange != null)
					{
						OnBeaconStateChange(this, new BeaconStateChangeEventArgs(false, beaconPosition));
					}
				}

				if (IsBeaconTracked)
				{
					lastPosition = tempLastPosition;

					// We rotate because the cameras on the headset are rotated.
					beaconPosition = Quaternion.Euler(stereoCamera.Settings.HeadsetSettings.CameraAngle, 0, 0) * beaconPosition;
					beaconPosition = beaconPosition + stereoCamera.Settings.HeadsetSettings.CameraOffset;
					beaconPosition.y -= BeaconCenterOffsetFromFloor;

					// This is how we "center" the world
					if (!sawBeacon)
					{
						logger.Log("Beacon Seen");
						sawBeacon = true;
						framesToCatchUp = 0;

						float angle = Vector2.Angle(Vector2.up, new Vector2(beaconPosition.x, beaconPosition.z).normalized);

						// Rotate the other way
						if (beaconPosition.x < 0)
						{
							angle *= -1;
						}

						// Rotate the camera to the new "center"
						stereoCamera.YDifference = -(stereoCamera.transform.rotation.eulerAngles.y - stereoCamera.YDifference) - angle;

						while (stereoCamera.YDifference > 360)
						{
							stereoCamera.YDifference -= 360;
						}

						while (stereoCamera.YDifference < 0)
						{
							stereoCamera.YDifference += 360;
						}

						// Factor in YDiff
						stereoCamera.SetCompassCorrection();
					}

					// Position Prediction
					if (Hmd.UsePositionPrediction)
					{
						beaconPosition = Hmd.GetPositionPrediction(beaconPosition);	
					}

					// Rotation Prediction
					if (Hmd.UseRotationPrediction)
					{
						// The predicted rotation difference.
						beaconPosition = rotationPrediction.GetRotationPrediction(stereoCamera.transform.rotation, stereoCamera.Settings.DeviceSettings.FrameDelay) * beaconPosition;

						// The difference from provided rotation and with prediction.
						beaconPosition += rotationPrediction.GetRotationPredictionOffset(stereoCamera.transform.rotation, stereoCamera.Settings.DeviceSettings.FrameDelay);
					}
					else
					{
						beaconPosition = stereoCamera.transform.rotation * beaconPosition;
					}

					// If we were not tracked last frame, lerp to the new position, else just set it.
					if (framesToCatchUp-- > 0)
					{
						stereoCamera.SetPosition(Vector3.Lerp(stereoCamera.transform.localPosition,
															  Vector3.zero - beaconPosition,
															  Time.deltaTime * 15));
					}
					else
					{
						float dist = Vector3.Distance(beaconPosition, lastPosition);
						float lerp = dist * dist * lerpFactor;
						lerp = Math.Max(0.3f, Math.Min(1.0f, lerp));

						if (lerp > .99f)
						{
							stereoCamera.SetPosition(Vector3.zero - beaconPosition);
						}
						else
						{
							stereoCamera.SetPosition(Vector3.Lerp(stereoCamera.transform.localPosition,
																  Vector3.zero - beaconPosition,
																  lerp));
						}
					}
				}
				else
				{
					// When you lose the beacon tracking, this will lerp to the new position so it doesn't snap
					framesToCatchUp = DurationToCatchTracked;
				}
			}

			#endregion

			#region Controllers

			foreach (Peripheral peripheral in connectionManager.Peripherals)
			{
				ControllerPeripheral controller = peripheral as ControllerPeripheral;

				// Not a controller
				if (controller == null)
				{
					continue;
				}

                //@EDIT:To Get Emulator Controller Input
#if UNITY_EDITOR
				// TODO: Fix emulation in UNITY EDITOR
                controller.IsTracked = true;
				controller.Position = new Vector3(0, 0, 0);//LenovoMirageARSDK.MirageAR_EditorEmulator.controllerPosition;
				controller.Rotation = Quaternion.identity;//LenovoMirageARSDK.MirageAR_EditorEmulator.controllerRotation;
#else

                controller.ControllerInput.UpdateState();

				// TODO: Ximmerse Bug
				// When you init the SDK you have to re-tell the controllers to set the tracked color, also when you first connect.
				// There is an issue in the SDK where you can't set the color right away after these events, but you have to wait a bit.
				// Like a second or two.
				if (controller.PendingColorSet && ++controller.PendingColorSetFrameCount > DelayForColorSet)
				{
					XDevicePlugin.SendMessage(controller.ControllerInput.handle, XDevicePlugin.kMessage_ChangeBlobColorDefault, controller.ColorBlobID, 0);
					controller.PendingColorSet = false;
					controller.PendingColorSetFrameCount = 0;
				}

				Vector3 position = new Vector3();
				TrackingResult result = XDevicePlugin.GetNodePosition(Hmd.ControllerInput.handle,
																	  0,
				                                                      controller.ColorBlobID,
																	  ref position);
                
				// Position Tracking
				if ((result & TrackingResult.PositionTracked) != 0)
				{
					// We rotate because the cameras on the headset are rotated.
					position = Quaternion.Euler(stereoCamera.Settings.HeadsetSettings.CameraAngle, 0, 0) * position;
					position = controller.GetPositionPrediction(position);
					position += stereoCamera.Settings.HeadsetSettings.CameraOffset;
					position = stereoCamera.transform.rotation * position;
					position = position + stereoCamera.transform.position;
					controller.Position = position;                    
					controller.IsTracked = true;
				}
				else
				{
					controller.IsTracked = false;
				}

				// Rotation Tracking
				Quaternion rotation = controller.ControllerInput.GetRotation();

                // Fix hardware issue of Kylo Controller,roll rotate 90 degrees，
                // Now it's a test version, and the offical version needs to remove "false"
                if (false&&peripheral.GetDeviceName().Contains("Kylo")){
                    rotation = rotation* Quaternion.Euler(0, 0, -90);
                }

				if (!rotation.x.Equals(0.0f))
				{
					rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y - (controller.YOffset - stereoCamera.YDifference), rotation.eulerAngles.z);
					controller.Rotation = rotation * Quaternion.Euler(0, 0, 180);
				}
#endif
            }

            #endregion
        }

		/// <summary>
		/// This should be called when the tracker instance is no longer needed.
		/// </summary>
		public void Destroy()
		{
			heartbeat.OnBeforeRender -= PreRender;
			heartbeat.OnFrameUpdate -= Update;

			OnBeaconStateChange = null;
		}

#endregion

#region Public Methods

		/// <summary>
		/// Start this tracking.
		/// </summary>
		public void StartTracking()
		{
			XDevicePlugin.Init();
		}

		/// <summary>
		/// Stop this tracking.
		/// </summary>
		public void StopTracking()
		{
			XDevicePlugin.Exit();
		}

		/// <summary>
		/// Pause the tracking.
		/// </summary>
		public void PauseTracking()
		{
			XDevicePlugin.OnPause();
		}

		/// <summary>
		/// Resume the tracking.
		/// </summary>
		public void ResumeTracking()
		{
			XDevicePlugin.OnResume();
		}

		/// <summary>
		/// Starts the pairing.
		/// </summary>
		public void StartPairing()
		{
			XDevicePlugin.StartBLEScan();
		}

		/// <summary>
		/// Stops the pairing.
		/// </summary>
		public void StopPairing()
		{
			XDevicePlugin.StopBLEScan();
		}

		/// <summary>
		/// Re-centers the forward vector for the player the next frame the beacon is seen.
		/// </summary>
		/// <param name="peripheralsToo">Whether to recenter all attached peripherals as well.</param>
		public void Recenter(bool peripheralsToo = false)
		{
			sawBeacon = false;

			if (peripheralsToo)
			{
				connectionManager.Peripherals.ForEach(item => 
				{
					if (item is ControllerPeripheral)
					{
						((ControllerPeripheral)item).Recenter(stereoCamera);
					}
				});
			}

		}

		/// <summary>
		/// Get the position of a given blob color with respect to the HMD.
		/// </summary>
		/// <returns>The position of the blob color</returns>
		/// <param name="color">The color of the blob to return the position of.</param>
		public Vector3 GetBlobPosition(int color)
		{
			Vector3 blobPosition = Vector3.zero;
			if (Hmd.ControllerInput.connectionState == DeviceConnectionState.Connected)
			{
				TrackingResult result = XDevicePlugin.GetNodePosition(Hmd.ControllerInput.handle, 0, color, ref blobPosition);
				if ((result & TrackingResult.PositionTracked) != 0)
				{
					blobPosition = Quaternion.Euler(stereoCamera.Settings.HeadsetSettings.CameraAngle, 0, 0) * blobPosition;
					blobPosition = blobPosition + stereoCamera.Settings.HeadsetSettings.CameraOffset;
				}
			}
			return blobPosition;
		}

#endregion
	}
}