using Ximmerse.Vision.Internal;
using UnityEngine;
using Ximmerse.InputSystem;
using AOT;
using System;
using System.Threading;

namespace Ximmerse.Vision
{
	/// <summary>
	/// Calibration state.
	/// </summary>
	public enum CalibrationState
	{
		Start = 0,
		InProgress1,
		InProgress2,
		Complete
	}

	public class ControllerPeripheral : Peripheral
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the color BLOB ID.
		/// </summary>
		/// <value>The color BLOB ID.</value>
		public int ColorBlobID { get; set; }

		/// <summary>
		/// If the peripherals light is visiable and being tracked.
		/// </summary>
		/// <value><c>true</c> if this instance is tracked; otherwise, <c>false</c>.</value>
		public bool IsTracked { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if we need to set the color after a delay.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public bool PendingColorSet { get; set; }

		/// <summary>
		/// Gets or sets the frame count before setting the color.
		/// </summary>
		/// <value>The pending color set frame count.</value>
		public int PendingColorSetFrameCount { get; set; }

		/// <summary>
		/// Gets or sets the yaw offset value that's applied to the rotation.
		/// </summary>
		/// <value>The yaw offset to use for the rotation.</value>
		public float YOffset { get; set; }

		#endregion

		#region Private Static Properties

		private static bool isCalibrating;
		private static int saberCalibrationState = -1;
		private static Action<CalibrationState> calibrationCallback;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.ControllerPeripheral"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="transform">Transform.</param>
		/// <param name="positionPrediction">Position prediction.</param>
		/// <param name="color">Color.</param>
		public ControllerPeripheral(string name, Transform transform = null, IPositionPrediction positionPrediction = null, ColorID color = ColorID.BLUE) : base(name, transform, positionPrediction)
		{
			ColorBlobID = (int)color;

			if (Connected)
			{
				SetColor();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="colorID">Color ID.</param>
		public void SetColor(int? colorID = null)
		{
			ColorBlobID = colorID ?? ColorBlobID;
			PendingColorSet = true;
		}

		/// <summary>
		/// Starts the calibration.
		/// </summary>
		/// <returns><c>true</c>, if calibration was started, <c>false</c> if another is currently calibrating.</returns>
		/// <param name="callback">Callback.</param>
		public bool StartCalibration(Action<CalibrationState> callback)
		{
			if (isCalibrating)
			{
				return false;
			}

			isCalibrating = true;
			saberCalibrationState = -1;
			calibrationCallback = callback;

			(new Thread(() => StartCalibrationThread(ControllerInput.handle))).Start();

			return true;
		}

		/// <summary>
		/// Stops calibrating.
		/// </summary>
		public void StopCalibrating()
		{
			if (isCalibrating)
			{
				isCalibrating = false;
				calibrationCallback = null;
				XDevicePlugin.SendMessage(ControllerInput.handle, XDevicePlugin.kMessage_SetChannel, 0, -1);
			}
		}

		/// <summary>
		/// Vibrate the peripheral with the specified strength for the specified duration.
		/// </summary>
		/// <param name="strength">The strength to vibrate.</param>
		/// <param name="duration">The duration to vibrate.</param>
		public void Vibrate(int strength = 75, float duration = 0.1f)
		{
			strength = Mathf.Clamp(strength, 0, 100);
			XDevicePlugin.SendMessage(ControllerInput.handle, XDevicePlugin.kMessage_TriggerVibration, strength, (int)(duration * 1000));
		}

		/// <summary>
		/// Zero out the yaw rotation value with respect to the passed in stereo camera object.
		/// </summary>
		/// <param name = "stereoCamera"></param>
		public void Recenter(StereoCamera stereoCamera)
		{
			ControllerInput.Recenter();
			YOffset = -stereoCamera.transform.rotation.eulerAngles.y + stereoCamera.YDifference;
		}

		#endregion

		#region Private Methods

		private static void StartCalibrationThread(int handle)
		{
			// We have to wait 100ms on Android between SendMessage Calls
			XDevicePlugin.NativeMethods.XDeviceSetCalibrationCallback(null);
			XDevicePlugin.SendMessage(handle, XDevicePlugin.kMessage_SetChannel, 0, 1);
			Thread.Sleep(100);
			XDevicePlugin.SendMessage(handle, XDevicePlugin.kMessage_SetCalibration, 0, 0);
			XDevicePlugin.NativeMethods.XDeviceSetCalibrationCallback(OnCalibrationResult);
		}

		[MonoPInvokeCallback(typeof(XDevicePlugin.calibration_delegate))]
		private static void OnCalibrationResult(int which, int state, ref XDevicePlugin.IMUCalibrationResult result)
		{
			// Not started yet.
			if (saberCalibrationState < 0 && state != 0)
			{
				return;
			}

			if (state != saberCalibrationState)
			{
				// checking for final state
				if (state == 3 && saberCalibrationState == 2)
				{
					XDevicePlugin.SendMessage(which, XDevicePlugin.kMessage_SetChannel, 0, -1);
				}

				// dispatching event since state has changed
				if (calibrationCallback != null)
				{
					calibrationCallback.Invoke((CalibrationState)state);
				}
			}

			saberCalibrationState = state;
		}

		#endregion
	}
}