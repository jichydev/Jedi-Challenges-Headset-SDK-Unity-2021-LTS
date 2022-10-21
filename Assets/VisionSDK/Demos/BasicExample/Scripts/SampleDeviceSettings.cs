using Ximmerse.Vision;
using Ximmerse.Vision.Internal;
using UnityEngine;
using UnityEngine.UI;
using System;
using Ximmerse.InputSystem;
//using LenovoMirageARSDK;
using System.Collections;

namespace Ximmerse.Example
{
	public class SampleDeviceSettings : MonoBehaviour
	{
		#region Public Properties

		public enum Side
		{
			Top,
			Left,
			Right,
			Bottom
		}

		// The controllers transform.
		public Transform Controller;

		// The Vision SDK
		public VisionSDK Sdk;

		public RectTransform SizerPanel;
		public RectTransform CanvasRect;
		public Image[] Borders;

		public Text DisplayText;
		public Text DeviceText;
		public Text DeviceName;

		public Side SelectedSide = Side.Top;

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

			// Setup Current Hardware Settings
			SetupCurrentSettings();
		}

		private void OnDestroy()
		{
			Sdk.Input.OnButtonDown -= OnButtonDown;
		}

		#endregion

		#region Public Methods

		public void UpdateSide(bool positive)
		{
			float change = (positive) ? -2.0f : 2.0f;

			switch (SelectedSide)
			{
				case Side.Top:
					SizerPanel.anchoredPosition = new Vector2(SizerPanel.anchoredPosition.x, SizerPanel.anchoredPosition.y - change);
					SizerPanel.sizeDelta = new Vector2(SizerPanel.sizeDelta.x, SizerPanel.sizeDelta.y - change);
				break;

				case Side.Bottom:
					SizerPanel.sizeDelta = new Vector2(SizerPanel.sizeDelta.x, SizerPanel.sizeDelta.y + -change);
				break;

				case Side.Left:
					SizerPanel.anchoredPosition = new Vector2(SizerPanel.anchoredPosition.x + change, SizerPanel.anchoredPosition.y);
					SizerPanel.sizeDelta = new Vector2(SizerPanel.sizeDelta.x - change, SizerPanel.sizeDelta.y);
				break;

				case Side.Right:
					SizerPanel.sizeDelta = new Vector2(SizerPanel.sizeDelta.x + -change, SizerPanel.sizeDelta.y);
				break;
			}

			UpdateDisplay();
		}

		public void NextSide()
		{
			SelectedSide++;

			if ((int)SelectedSide > 3)
			{
				SelectedSide = 0;
			}

			foreach (Image border in Borders)
			{
				border.color = Color.white;
			}

			GameObject.Find(SelectedSide.ToString()).GetComponent<Image>().color = Color.green;
		}

		public void ApplySettings()
		{
			UpdateDisplay(true);
		}

		public void LogJson()
		{
			UpdateDisplay(false, true);
		}

		public void HideUI()
		{
			CanvasRect.gameObject.SetActive(!CanvasRect.gameObject.activeSelf);
		}

		#endregion

		#region Private Methods

		private void SetupCurrentSettings()
		{
			// Percents
			// Sdk.Settings.DeviceSettings

			float top = (1.0f - Sdk.Settings.DeviceSettings.UITopOffset) * CanvasRect.sizeDelta.y;
			float bottom = Sdk.Settings.DeviceSettings.UIBottomOffset * CanvasRect.sizeDelta.y;
			float left = (1.0f - Sdk.Settings.DeviceSettings.UILeftOffset) * CanvasRect.sizeDelta.x;
			float right = Sdk.Settings.DeviceSettings.UIRightOffset * CanvasRect.sizeDelta.x;

			SizerPanel.anchoredPosition = new Vector2(left, -top);
			SizerPanel.sizeDelta = new Vector2(right - left, bottom - top);

			UpdateDisplay();
		}

		private void OnButtonDown(object sender, ButtonEventArgs eventArguments)
		{
			Sdk.Logger.Log(eventArguments.Peripheral.Name + " (D): " + eventArguments.Button);

			CanvasRect.gameObject.SetActive(true);

			if (eventArguments.Button == Vision.ButtonType.HmdBack)
			{
				UpdateSide(true);
			}
			else if (eventArguments.Button == Vision.ButtonType.HmdMenu)
			{
				UpdateSide(false);
			}
			else if (eventArguments.Button == Vision.ButtonType.HmdSelect)
			{
				NextSide();
			}
			else if (eventArguments.Button == Vision.ButtonType.SaberActivate)
			{
				LogJson();
			}
			else if (eventArguments.Button == Vision.ButtonType.SaberControl)
			{
				ApplySettings();
			}
		}

		private void UpdateDisplay(bool apply = false, bool log = false)
		{
			float top = 1.0f - (Mathf.Abs(SizerPanel.anchoredPosition.y) / CanvasRect.sizeDelta.y);
			float left = 1.0f - (SizerPanel.anchoredPosition.x / CanvasRect.sizeDelta.x);
			float bottom = 1.0f - ((CanvasRect.sizeDelta.y - SizerPanel.sizeDelta.y - Mathf.Abs(SizerPanel.anchoredPosition.y)) / CanvasRect.sizeDelta.y);
			float right = 1.0f - ((CanvasRect.sizeDelta.x - SizerPanel.sizeDelta.x - SizerPanel.anchoredPosition.x) / CanvasRect.sizeDelta.x);

			top = (float)Math.Round((Decimal)top, 3, MidpointRounding.AwayFromZero);
			left = (float)Math.Round((Decimal)left, 3, MidpointRounding.AwayFromZero);
			bottom = (float)Math.Round((Decimal)bottom, 3, MidpointRounding.AwayFromZero);
			right = (float)Math.Round((Decimal)right, 3, MidpointRounding.AwayFromZero);

			if (apply)
			{
				DeviceSettings settings = new DeviceSettings();
				settings.UITopOffset = top;
				settings.UILeftOffset = left;
				settings.UIRightOffset = right;
				settings.UIBottomOffset = bottom;

				Sdk.Settings.DeviceSettings = settings;
			}

			if (log)
			{
				Debug.Log(JsonUtility.ToJson(Sdk.Settings.DeviceSettings, true));
			}

			string result = "";
			result += "Top: " + top + "\n";
			result += "Left: " + left + "\n";
			result += "Right: " + right + "\n";
			result += "Bottom: " + bottom + "\n";

			DisplayText.text = result;

			Vector2 dpi = SettingsManager.GetXYDpi();
			DeviceText.text = "X-DPI: " + dpi.x + "\nY-DPI: " + dpi.x + "\nWidth: " + Screen.width + "\nHeight: " + Screen.height;
			DeviceName.text = SystemInfo.deviceModel;
		}

		#endregion
	}
}