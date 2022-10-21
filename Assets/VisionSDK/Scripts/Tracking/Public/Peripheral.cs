using UnityEngine;
using Ximmerse.Vision.Internal;
using Ximmerse.InputSystem;

namespace Ximmerse.Vision
{
	public enum ColorID
	{
		RED = 0,
		GREEN = 1,
		CYAN = 2,
		BLUE = 3,
		PURPLE = 4,
		PINK = 5,
	}

	public class Peripheral
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the name of the peripheral.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// If we should use the position prediction for this peripheral
		/// </summary>
		/// <value><c>true</c> if use position prediction; otherwise, <c>false</c>.</value>
		public bool UsePositionPrediction { get; set; }

		/// <summary>
		/// The Position Prediction to use if UsePositionPrediction is true
		/// </summary>
		/// <value>The position prediction.</value>
		public IPositionPrediction PositionPrediction { get; set; }

		/// <summary>
		/// If this Peripheral is connected.
		/// </summary>
		/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
		public bool Connected { get; set; }

		/// <summary>
		/// INTERNAL USE ONLY
		/// </summary>
		/// <value>The controller input.</value>
		public ControllerInput ControllerInput { get; set; }

		/// <summary>
		/// Gets or sets the position of the peripheral.
		/// </summary>
		/// <value>The position.</value>
		public Vector3 Position
		{
			get
			{
				return position;
			}

			set
			{
				position = value;
				if (transform != null)
				{
					transform.localPosition = position;
				}
			}
		}

		/// <summary>
		/// Gets or sets the rotation of the peripheral.
		/// </summary>
		/// <value>The rotation.</value>
		public Quaternion Rotation
		{
			get
			{
				return rotation;
			}

			set
			{
				rotation = value;
				if (transform != null)
				{
					transform.localRotation = rotation;
				}
			}
		}

		#endregion

		#region Private Properties

		private Transform transform;
		private Vector3 position;
		private Quaternion rotation;

		#endregion

		#region Constructor

		public Peripheral(string name, Transform transform = null, IPositionPrediction positionPrediction = null)
		{
			Name = name;
			PositionPrediction = positionPrediction ?? new PositionPrediction();

			if (!string.IsNullOrEmpty(Name))
			{
				ControllerInput = new ControllerInput(XDevicePlugin.GetInputDeviceHandle(Name));
				Connected = (ControllerInput.connectionState == DeviceConnectionState.Connected);
			}
			this.transform = transform;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		public virtual void Destroy()
		{
			transform = null;
			Name = "";
			PositionPrediction = null;
			ControllerInput = null;
			Connected = false;
		}

		/// <summary>
		/// Gets the position based on prediction.
		/// </summary>
		/// <returns>The position prediction.</returns>
		/// <param name="position">Position.</param>
		public virtual Vector3 GetPositionPrediction(Vector3 position)
		{
			return (UsePositionPrediction && PositionPrediction != null) ? PositionPrediction.GetPrediction(position) : position;
		}

		/// <summary>
		/// Gets the name of the device.
		/// </summary>
		/// <returns>The device name.</returns>
		public virtual string GetDeviceName()
		{
			return XDevicePlugin.GetString(ControllerInput.handle, XDevicePlugin.kField_DisplayNameObject, "");
		}

		/// <summary>
		/// Gets the name of the model.
		/// </summary>
		/// <returns>The model name.</returns>
		public virtual string GetModelName()
		{
			return XDevicePlugin.GetString(ControllerInput.handle, XDevicePlugin.kField_ModelNameObject, "");
		}

		/// <summary>
		/// Gets the serial number.
		/// </summary>
		/// <returns>The serial number.</returns>
		public virtual string GetSerialNumber()
		{
			return XDevicePlugin.GetString(ControllerInput.handle, XDevicePlugin.kField_SerialNumberObject, "");
		}

		/// <summary>
		/// Gets the firmware version.
		/// </summary>
		/// <returns>The firmware version.</returns>
		public virtual string GetFirmwareVersion()
		{
			return XDevicePlugin.GetString(ControllerInput.handle, XDevicePlugin.kField_FirmwareRevisionObject, "");
		}

		/// <summary>
		/// Gets the battery level.
		/// </summary>
		/// <returns>The battery level.</returns>
		public virtual int GetBatteryLevel()
		{
			return ControllerInput.batteryLevel;
		}

		#endregion
	}
}