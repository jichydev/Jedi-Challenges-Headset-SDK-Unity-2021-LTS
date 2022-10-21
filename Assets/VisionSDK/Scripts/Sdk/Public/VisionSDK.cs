using Ximmerse.Vision.Internal;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Collections.Generic;


namespace Ximmerse.Vision
{
	public class VisionSDK : MonoBehaviour
	{
        #region Public Static

        //@EDIT:Add Instance
        /// <summary>
        /// the instance
        /// </summary>
        public static VisionSDK Instance;

        /// <summary>
        /// The VisionSDK version string.
        /// </summary>
        public static string Version = "2.0.1";         //Vision SDK Version

        //@EDIT:update xdevice-api.aar to 3.0.3.8
        /// <summary>
        /// The ximmerse android library version.
        /// </summary>
        public static string XimmerseAndroidLibraryVersion = "3.0.3.8"; // ximmser Android Library Version

        /// <summary>
        /// The ximmerse iOS library version.
        /// </summary>
        public static string XimmerseiOSLibraryVersion = "3.0.3.8"; // ximmser iOS Library Version

        #endregion

        #region Public Inspector Properties

        /// <summary>
        /// The heartbeat instance (MonoBehaviour).
        /// </summary>
        public Heartbeat Heartbeat;

		/// <summary>
		/// The stereo camera instance (MonoBehaviour).
		/// </summary>
		public StereoCamera StereoCamera;

		/// <summary>
		/// Nodes to track
		/// </summary>
		public List<VisionTrackedNode> TrackedNodes = new List<VisionTrackedNode>();

		/// <summary>
		/// If you want to force single camera in editor.
		/// </summary>
		public bool ForceSingleCameraInEditor = true;

		#endregion

		#region Public Systems

		/// <summary>
		/// If the SDK is inited
		/// </summary>
		/// <value><c>true</c> if inited; otherwise, <c>false</c>.</value>
		public bool Inited { get; private set; }

		/// <summary>
		/// The connection manager.
		/// </summary>
		/// <value>The connections.</value>
		public IConnectionManager Connections { get; private set; }

		/// <summary>
		/// A reference to the Ximmerse tracker <see cref="Ximmerse.Vision.IXimmerseTracker"/>.
		/// </summary>
		/// <value>The instance of the tracker.</value>
		public IXimmerseTracker Tracking { get; private set; }

		/// <summary>
		/// A reference to the logger <see cref="Ximmerse.Vision.IVisionLogger"/>.
		/// </summary>
		/// <value>The instance of the logger.</value>
		public IVisionLogger Logger { get; private set; }

		/// <summary>
		/// A reference to the input manager <see cref="Ximmerse.Vision.IInputManager"/>.
		/// </summary>
		/// <value>The instance of the input manager.</value>
		public IInputManager Input { get; private set; }

		/// <summary>
		/// Device manager
		/// </summary>
		/// <value>The devices.</value>
		public ISettingsManager Settings { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Init the SDK with the camera type and a logger.
		/// </summary>
		/// <param name="singleCamera">If set to <c>true</c> single camera.</param>
		/// <param name="visionLogger">Vision logger.</param>
		/// <param name="settingsManager">Settings manager.</param>
		/// <param name="flyCamera">If set to <c>true</c> use fly camera in editor.</param>
		public void Init(bool singleCamera,
		                 IVisionLogger visionLogger = null,
		                 ISettingsManager settingsManager = null,
		                 bool flyCamera = true)
		{
			// The camera type
			SetCameraType(singleCamera);
			
            //@EDIT:Remove FlyCamera,Use LenovoEditorEmulator.cs to Emulat Controller and Hmd Input
			//if (Application.isEditor && flyCamera)
			//{
            //gameObject.AddComponent<FlyCamera>();
			//}

			// Setup the systems
			Logger = visionLogger ?? new VisionLogger();
			Logger.Log("Logger initialized.");
			Settings = settingsManager ?? new SettingsManager(Logger);
			Connections = new ConnectionManager(Heartbeat);
			Tracking = new XimmerseTracker(Logger, Heartbeat, StereoCamera, Connections);           
            Input = new InputManager(Heartbeat, Connections);      

            StereoCamera.Settings = Settings;
			StereoCamera.UpdateCameraSettings();

			// Events
			Settings.OnSettingsChanged += OnSettingsChanged;
			Heartbeat.OnBeforeRender += OnBeforeRender;

			// SDK Ready
			Inited = true;
		}

        //@EDIT: Add Unload fuction
        /// <summary>
        /// Unload SDK
        /// </summary>
        public void UnLoad()
        {
            if (!Inited)
            {
                return;
            }

            Settings.OnSettingsChanged -= OnSettingsChanged;
            Heartbeat.OnBeforeRender -= OnBeforeRender;

            Tracking.Destroy();
            Input.Destroy();
            Logger.Destroy();
            Connections.Destroy();
            Settings.Destroy();
        }

        #endregion

        #region Unity Methods

        private void Awake()
		{
            //@EDIT:Set the Instance value
            if (Instance == null)
            {
                Instance = this;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		private void OnApplicationPause(bool state)
		{
			if (!Inited)
			{
				return;
			}

			if (state)
			{
				#if UNITY_ANDROID
				Tracking.PauseTracking();
				#else
				Tracking.StopTracking();
				#endif
			}
			else
			{
				#if UNITY_ANDROID
				Tracking.ResumeTracking();
				#else
				Tracking.StartTracking();
				#endif
			}
		}

		private void OnApplicationQuit()
		{
			if (!Inited)
			{
				return;
			}

			Tracking.StopTracking();
		}

		private void OnDestroy()
		{
			if (!Inited)
			{
				return;
			}

			Settings.OnSettingsChanged -= OnSettingsChanged;
			Heartbeat.OnBeforeRender -= OnBeforeRender;

			Tracking.Destroy();
			Input.Destroy();
			Logger.Destroy();
			Connections.Destroy();
			Settings.Destroy();
		}

		#endregion

		#region Private Methods

		private void SetCameraType(bool singleCamera)
		{
			if (Application.isEditor && ForceSingleCameraInEditor)
			{
				singleCamera = true;
			}

			StereoCamera.LeftCamera.gameObject.SetActive(!singleCamera);
			StereoCamera.RightCamera.gameObject.SetActive(!singleCamera);
			StereoCamera.BottomCamera.gameObject.SetActive(!singleCamera);
			StereoCamera.SingleCamera.gameObject.SetActive(singleCamera);
			Heartbeat = StereoCamera.GetHeartbeat();

			// This enables "magic window" which is the only way we can get orientation from GVR and keep our rendering setup
			#if !UNITY_EDITOR
			XRSettings.enabled = false;
			XRSettings.LoadDeviceByName((singleCamera) ? "none" : "cardboard");
			#endif
		}

		private void OnSettingsChanged(object sender, SettingsChangeEventArgs eventArgs)
		{
			StereoCamera.UpdateCameraSettings();
		}

		private void OnBeforeRender(object sender, EventArgs eventArgs)
		{
			for (int i = 0; i < TrackedNodes.Count; i++)
			{
				if (TrackedNodes[i] == null)
				{
					continue;
				}

				switch (TrackedNodes[i].NodeType)
				{
				// Head
					case VisionNode.Head:
						TrackedNodes[i].transform.localPosition = StereoCamera.transform.localPosition;
						TrackedNodes[i].transform.localRotation = StereoCamera.transform.localRotation;
						break;

				// Controller
					case VisionNode.Controller:
						Peripheral controller = Connections.GetPeripheral(TrackedNodes[i].PeripheralName);                        

						if (controller != null)
						{
                            //@EDIT:
                            //  1,To Sync Controller World Position And Rotation,Not Local Position And Rotation
                            //Original----------------------------------------
                            //TrackedNodes[i].transform.localPosition = controller.Position;
                            //TrackedNodes[i].transform.localRotation = controller.Rotation;
                            //Original----------------------------------------
                            //Now----------------------------------------
                            TrackedNodes[i].transform.position = controller.Position;
                            TrackedNodes[i].transform.rotation = controller.Rotation;
                            //Now----------------------------------------
                        }
                        break;

				// Beacon
					case VisionNode.Beacon:
						TrackedNodes[i].transform.localPosition = Vector3.zero;
						TrackedNodes[i].transform.localRotation = Quaternion.identity;
						break;
				}
			}
		}

#endregion
	}
}
