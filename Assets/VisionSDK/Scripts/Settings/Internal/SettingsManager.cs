using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Ximmerse.Vision.Internal;

namespace Ximmerse.Vision
{
	/// <summary>
	/// The Setting type.
	/// </summary>
	public enum SettingType
	{
		Headset,
		Device
	}

	public class SettingsManager : ISettingsManager
	{
		#region Static Public Properties

		// TODO: Do we have a way to detect hardware? Not before the HMD is plugged in though..
		public static string HeadsetName = "PVT";

		#endregion

		#region Public Events

		/// <summary>
		/// Event for when the device or headset Settings being used are changed.
		/// </summary>
		/// <value>The on settings changed.</value>
		public EventHandler<SettingsChangeEventArgs> OnSettingsChanged { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the headset settings.
		/// </summary>
		/// <value>The headset settings.</value>
		public HeadsetSettings HeadsetSettings
		{
			get
			{
				return headsetSettings;
			}
			set
			{
				headsetSettings = value;

				if (OnSettingsChanged != null)
				{
					OnSettingsChanged(this, new SettingsChangeEventArgs(SettingType.Headset));
				}
			}
		}

		/// <summary>
		/// Gets or sets the device settings.
		/// </summary>
		/// <value>The device settings.</value>
		public DeviceSettings DeviceSettings
		{
			get
			{
				return deviceSettings;
			}
			set
			{
				deviceSettings = value;

				if (OnSettingsChanged != null)
				{
					OnSettingsChanged(this, new SettingsChangeEventArgs(SettingType.Device));
				}
			}
		}

		#endregion

		#region Private Properties

		// Private Settings
		private DeviceSettings deviceSettings;
		private HeadsetSettings headsetSettings;

		// Setting Data
		private readonly IVisionLogger logger;
		private readonly Devices deviceList;
		private readonly HeadsetList headsetList;
		private readonly string platformName;

		// Editor Values
		private const string EditorPlatform = "Android";
		private const string EditorDeviceModel = "samsung SC-02H";

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.SettingsManager"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		public SettingsManager(IVisionLogger logger)
		{
			this.logger = logger;

			// Hardware
			TextAsset headsets = Resources.Load<TextAsset>("Profiles/headset_list");
			if (headsets == null)
			{
				this.logger.Log("Error: Fail to load hardware list");
				return;
			}

			headsetList = JsonUtility.FromJson<HeadsetList>(headsets.text);

			HeadsetSettings = GetRecomendedSettingsForHeadset();

			// Devices
			TextAsset devices = Resources.Load<TextAsset>("Profiles/devices");
			if (devices == null)
			{
				this.logger.Log("Error: Fail to load device list");
				return;
			}

			platformName = (Application.platform == RuntimePlatform.IPhonePlayer) ? "iOS" : "Android";
			deviceList = JsonUtility.FromJson<Devices>(devices.text);

			#if UNITY_EDITOR
			platformName = EditorPlatform;
			#endif

			DeviceSettings = GetRecomendedSettingsForDevice();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		public void Destroy()
		{
			OnSettingsChanged = null;
		}

		/// <summary>
		/// Gets the recomended settings for headset.
		/// </summary>
		/// <returns>The recomended settings for headset.</returns>
		public HeadsetSettings GetRecomendedSettingsForHeadset()
		{
			HeadsetSettings settings = headsetList.Headsets.Find(item => item.Name.Contains(HeadsetName));

			if (settings == null)
			{
				logger.Log("Error: Failed to load Recommended Headset Settings");

				if (headsetList.Headsets.Count > 0)
				{
					settings = headsetList.Headsets[0];
				}
				else
				{
					logger.Log("Error: No Headset Settings Found");
					settings = new HeadsetSettings();
				}
			}

			return settings;
		}

		/// <summary>
		/// Gets the recomended settings for device.
		/// </summary>
		/// <returns>The recomended settings for device.</returns>
		/// <param name="deviceName">Device name.</param>
		public DeviceSettings GetRecomendedSettingsForDevice(string deviceName = null)
		{
			string model = deviceName ?? SystemInfo.deviceModel;

			#if UNITY_EDITOR
			model = EditorDeviceModel;
			#endif

			// Check if the model is in the list of device names
			List<Device> devices = deviceList.Platforms.Single(item => item.Platform == platformName).Devices;

			Device device = devices.Find(item => item.Models.Find(modelName => modelName.Equals(model)) != null);

			device = device ?? devices.Find(item => item.BaseModelNames.Find(modelName => modelName.Contains(model)) != null);

			// No device found matching the model.
			if (device == null)
			{
				if (devices.Count > 0)
				{
					device = devices[0];
				}
				else
				{
					logger.Log("Error: Device Not Found");
					return new DeviceSettings();
				}
			}

			TextAsset profile = Resources.Load<TextAsset>("Profiles/" + device.SettingsFile.Replace(".txt", ""));

			// No profile found for the given device.
			if (profile == null)
			{
				logger.Log("Error: Profile Not Found");
				return new DeviceSettings();
			}

			return JsonUtility.FromJson<DeviceSettings>(profile.text);
		}

		public static Vector2 GetXYDpi()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaObject metrics = new AndroidJavaObject("android.util.DisplayMetrics");
			activity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call("getMetrics", metrics);

			return new Vector2(metrics.Get<float>("xdpi"), metrics.Get<float>("ydpi"));
			#else
			return new Vector2(Screen.dpi, Screen.dpi);
			#endif
		}

		#endregion
	}
}