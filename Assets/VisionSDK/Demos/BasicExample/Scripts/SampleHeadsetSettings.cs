using Ximmerse.Vision;
using Ximmerse.Vision.Internal;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;
using Ximmerse.InputSystem;
//using LenovoMirageARSDK;
using System.Collections;

namespace Ximmerse.Example
{
	public class SampleHeadsetSettings : MonoBehaviour
	{
		#region Public Properties

		// The controllers transform.
		public Transform Controller;

		// The Vision SDK
		public VisionSDK Sdk;

		public Text ValueText;
		public Text SettingName;

		#endregion

		#region Private Properties 

		private List<FieldInfo> properties = new List<FieldInfo>();
		private FieldInfo currentProperty;
		private int currentPropertyIndex;
		private HeadsetSettings settings;

        #endregion

        #region Unity Methods

        //@EDIT:
        private void Awake()
        {
        }

		//@EDIT:
		private void Start()
		{
			Sdk.Init(false);

			// If you want to use prediction for the HMD (Phone).
			Sdk.Tracking.Hmd.UsePositionPrediction = true;
			Sdk.Tracking.Hmd.UseRotationPrediction = true;

			// Setup Controllers with a Transform if one
			ControllerPeripheral controller = new ControllerPeripheral("XCobra-0", Controller);
			controller.UsePositionPrediction = true;

			Sdk.Connections.AddPeripheral(controller);

			// Input Events
			Sdk.Input.OnButtonDown += OnButtonDown;

			// New Settings
			settings = Sdk.Settings.HeadsetSettings;

			FieldInfo[] fields = typeof(HeadsetSettings).GetFields();

			for (int i = 0; i < fields.Length; i++)
			{
				// Only Floats
				if (fields[i].FieldType == typeof(System.Single))
				{
					properties.Add(fields[i]);
				}
			}

			currentProperty = properties[0];
			currentPropertyIndex = 0;

			UpdateDisplay();
		}

		private void OnDestroy()
		{
			Sdk.Input.OnButtonDown -= OnButtonDown;
		}

		#endregion

		#region Public Methods

		public void ApplySettings()
		{
			UpdateDisplay(true);
		}

		public void LogJson()
		{
			UpdateDisplay(false, true);
		}

		#endregion

		#region Private Methods

		private void OnButtonDown(object sender, ButtonEventArgs eventArguments)
		{
			Sdk.Logger.Log(eventArguments.Peripheral.Name + " (D): " + eventArguments.Button);

			if (eventArguments.Button == Vision.ButtonType.HmdBack)
			{
				RangeAttribute range = (RangeAttribute)currentProperty.GetCustomAttributes(typeof(RangeAttribute), true)[0];
				currentProperty.SetValue(settings, (float)currentProperty.GetValue(settings) - ((range.max - range.min) * 0.02f));
			}
			else if (eventArguments.Button == Vision.ButtonType.HmdMenu)
			{
				RangeAttribute range = (RangeAttribute)currentProperty.GetCustomAttributes(typeof(RangeAttribute), true)[0];
				currentProperty.SetValue(settings, (float)currentProperty.GetValue(settings) + ((range.max - range.min) * 0.02f));
			}
			else if (eventArguments.Button == Vision.ButtonType.HmdSelect)
			{
				currentPropertyIndex++;

				if (currentPropertyIndex >= properties.Count)
				{
					currentPropertyIndex = 0;
				}

				currentProperty = properties[currentPropertyIndex];
			}
			else if (eventArguments.Button == Vision.ButtonType.SaberActivate)
			{
				LogJson();
			}
			else if (eventArguments.Button == Vision.ButtonType.SaberControl)
			{
				ApplySettings();
			}

			UpdateDisplay();
		}

		private void UpdateDisplay(bool apply = false, bool log = false)
		{
			if (apply)
			{
				Sdk.Settings.HeadsetSettings = settings;
			}

			if (log)
			{
				Debug.Log(JsonUtility.ToJson(Sdk.Settings.HeadsetSettings, true));
			}

			ValueText.text = "Value: " + currentProperty.GetValue(settings);
			SettingName.text = currentProperty.Name;
		}

		#endregion
	}
}