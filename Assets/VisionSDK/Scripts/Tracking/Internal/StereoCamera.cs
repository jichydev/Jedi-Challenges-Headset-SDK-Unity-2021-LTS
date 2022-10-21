using UnityEngine;

using UnityEngine.XR;

namespace Ximmerse.Vision.Internal
{
	public class StereoCamera : MonoBehaviour
	{
		#region Public Inspector Properties

		/// <summary>
		/// The left eye's cameras transform.
		/// </summary>
		public Camera LeftCamera;

		/// <summary>
		/// The right eye's cameras transform.
		/// </summary>
		public Camera RightCamera;

		/// <summary>
		/// The bottom camera transform.
		/// </summary>
		public Camera BottomCamera;

		/// <summary>
		/// Full screen camera
		/// </summary>
		public Camera SingleCamera;

		#endregion

		#region Public Properties

		/// <summary>
		/// Devices
		/// </summary>
		/// <value>New camera settings to update.</value>
		public ISettingsManager Settings { get; set; }

		/// <summary>
		/// Gets or sets the Y difference, used to center the player to the beacon.
		/// This is to keep the world centered to the player. YDifference is set the first time (and any recenter's) that they see the beacon
		/// </summary>
		/// <value>The Y difference.</value>
		public float YDifference { get; set; }

		/// <summary>
		/// If we should correct y rotation based on the compass.
		/// </summary>
		public bool UseMagnetometerCorrection { get; set; }

		#endregion

		#region Private Properties

		// Magnetometer Correction
		private CompassManager compassManager;

		// compass correction related
		private double lastCompassUpdateTime = 0;
		private Quaternion targetCompassCorrection;
		private Quaternion compassCorrection;
		private Quaternion compassOrientationDelta;
		private bool correctionInitialized = false;

		#if UNITY_ANDROID && !UNITY_EDITOR
		private bool androidOrientationSet = false;
		#endif

		#endregion

		#region Unity Events

		public void Start()
		{
			compassManager = new CompassManager();

			InputTracking.disablePositionalTracking = true;

			Input.gyro.enabled = true;
			Input.gyro.updateInterval = 0.01f;
			Input.compass.enabled = true;

			// Android Orientation Fix
			#if UNITY_ANDROID && !UNITY_EDITOR
			LeftCamera.enabled = false;
			RightCamera.enabled = false;
			SingleCamera.enabled = false;

			using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
					activity.Call("runOnUiThread", new AndroidJavaRunnable(RotationAnimationRunnable));
                }
            }
			#endif
		}

		public void Update()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (Screen.orientation != ScreenOrientation.LandscapeRight)
			{
				Screen.orientation = ScreenOrientation.LandscapeRight;
			}
			else if (!androidOrientationSet)
			{
				androidOrientationSet = true;
				LeftCamera.enabled = true;
				RightCamera.enabled = true;
				SingleCamera.enabled = true;
			}
			#else
			Screen.orientation = ScreenOrientation.LandscapeRight;
			#endif
		}

		public void OnDestroy()
		{

		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the heartbeat.
		/// </summary>
		/// <returns>The heartbeat.</returns>
		public Heartbeat GetHeartbeat()
		{
			Heartbeat heartbeat = null;
			if (RightCamera.gameObject.activeInHierarchy)
			{
				heartbeat = RightCamera.GetComponent<Heartbeat>();
			}
			if (heartbeat == null)
			{
				heartbeat = SingleCamera.GetComponent<Heartbeat>();
			}

			return heartbeat;
		}

		/// <summary>
		/// Updates the rotation.
		/// </summary>
		/// <param name="instant">Update to the target correction fully instead of lerping.</param>
		public void UpdateRotation(bool instant = false)
		{
			Quaternion gyroOrientation = GetGyroOrientation();
			Vector3 newRotation = gyroOrientation.eulerAngles;

			if (UseMagnetometerCorrection && correctionInitialized)
			{
				// See if the compass has new data
				if (Input.compass.timestamp > (lastCompassUpdateTime + 0.10f))
				{
					lastCompassUpdateTime = Input.compass.timestamp;

					// Calculate the target correction factor
					targetCompassCorrection = compassManager.GetTargetCorrection(gyroOrientation);
				}

				compassCorrection = Quaternion.Slerp(compassCorrection, targetCompassCorrection, (instant) ? 1.0f : 0.17f);

				newRotation.y += YDifference + (compassCorrection * compassOrientationDelta).eulerAngles.y;
			}
			else
			{
				newRotation.y += YDifference;
			}

			#if !UNITY_EDITOR
			transform.rotation = Quaternion.Euler(newRotation);
			#endif
		}

		/// <summary>
		/// Sets the position of the cameras.
		/// </summary>
		/// <param name="position">The position.</param>
		public void SetPosition(Vector3 position)
		{
			#if !UNITY_EDITOR
			transform.localPosition = position;
			#endif
		}

		/// <summary>
		/// Updates the camera settings.
		/// </summary>
		public void UpdateCameraSettings()
		{
			//adjust the camera rects
			float cameraWidth = (1.0f - (1.0f - Settings.DeviceSettings.UIRightOffset + 1.0f - Settings.DeviceSettings.UILeftOffset)) / 2.0f;
			float cameraHeight = 1.0f - (1.0f - Settings.DeviceSettings.UITopOffset + 1.0f - Settings.DeviceSettings.UIBottomOffset);
			float cameraX = 1.0f - Settings.DeviceSettings.UILeftOffset;
			float cameraY = 1.0f - Settings.DeviceSettings.UIBottomOffset;

			LeftCamera.rect = new Rect(cameraX, cameraY, cameraWidth, cameraHeight);
			RightCamera.rect = new Rect(cameraX + cameraWidth, cameraY, cameraWidth, cameraHeight);
			BottomCamera.rect = new Rect(0f, 0f, 1f, 1f);

			LeftCamera.depth = 1;
			RightCamera.depth = 0;
			BottomCamera.depth = -1;

			// Update Eye position
			LeftCamera.transform.localPosition = new Vector3(-Settings.HeadsetSettings.Ipd / 2.0f, 0.0f, 0.0f);
			RightCamera.transform.localPosition = new Vector3(Settings.HeadsetSettings.Ipd / 2.0f, 0.0f, 0.0f);

			// Update Eye Field of View
			LeftCamera.fieldOfView = Settings.HeadsetSettings.Fov;
			RightCamera.fieldOfView = Settings.HeadsetSettings.Fov;

			// Update Screen Coordinates
			float screenOffsetHorizontalLeft = -Settings.HeadsetSettings.HorizontalOffset + Settings.HeadsetSettings.HorizontalSplit;
			float screenOffsetHorizontalRight = -Settings.HeadsetSettings.HorizontalOffset - Settings.HeadsetSettings.HorizontalSplit;
			float screenOffsetVerticalLeft = -Settings.HeadsetSettings.VerticalOffset + Settings.HeadsetSettings.VerticalSplit;
			float screenOffsetVerticalRight = -Settings.HeadsetSettings.VerticalOffset - Settings.HeadsetSettings.VerticalSplit;

			LeftCamera.ResetProjectionMatrix();
			RightCamera.ResetProjectionMatrix();

			UpdateScreenCoordinates(LeftCamera, screenOffsetHorizontalLeft, screenOffsetVerticalLeft);
			UpdateScreenCoordinates(RightCamera, screenOffsetHorizontalRight, screenOffsetVerticalRight);
		}

		/// <summary>
		/// Sets the target compass correction.
		/// </summary>
		public void SetCompassCorrection()
		{
			targetCompassCorrection = compassCorrection = compassManager.GetTargetCorrection(GetGyroOrientation());
			compassOrientationDelta = Quaternion.Inverse(compassCorrection);
			correctionInitialized = true;

			UpdateRotation(true);
		}

		#endregion

		#region Private Static Methods

		private static Quaternion GetGyroOrientation()
		{
			Quaternion cardboardRotation = InputTracking.GetLocalRotation(XRNode.Head);

			#if UNITY_ANDROID
			cardboardRotation = cardboardRotation * Quaternion.Euler(0, 0, 180);
			#endif

			Quaternion gyroOrientation = Quaternion.identity;

			// Adjust Unity output quaternion as per Android SensorManager
			gyroOrientation.w = cardboardRotation.w;
			gyroOrientation.x = -cardboardRotation.x;
			gyroOrientation.y = cardboardRotation.z;
			gyroOrientation.z = cardboardRotation.y;

			// Without VR enabled, cardboard has big difference in settings
			// change axis around to accomodate the mag sensor orientation and declination
			return Quaternion.Euler(-90f, 0f, 0f) * gyroOrientation; 
		}

		private static void UpdateScreenCoordinates(Camera eyeCamera, float horizontalOblique, float verticalOblique)
		{
			eyeCamera.ResetProjectionMatrix();

			// Update the eye camera to use oblique style projection (render 3d as 2d)
			Matrix4x4 matrix = eyeCamera.projectionMatrix;
			matrix[0,2] = horizontalOblique;
			matrix[1,2] = verticalOblique;

			eyeCamera.projectionMatrix = matrix;
		}

		#if UNITY_ANDROID && !UNITY_EDITOR
		/// <summary>
		/// Sets the animation when changing orientation to be instant.
		/// </summary>
		private static void RotationAnimationRunnable()
		{
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					using (AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow"))
					{
						using (AndroidJavaObject layoutParam = window.Call<AndroidJavaObject>("getAttributes"))
						{
							layoutParam.Set<int>("rotationAnimation", 2);
							window.Call("setAttributes", layoutParam);
						}
					}
				}
			}
			Screen.orientation = ScreenOrientation.LandscapeRight;
		}
		#endif

		#endregion
	}
}