using Ximmerse.InputSystem;

namespace Ximmerse.Vision
{
	public class HmdPeripheral : Peripheral
	{
		#region Public Properties

		/// <summary>
		/// The Controller Input for input (hmd buttons).
		/// </summary>
		public ControllerInput Input;

		/// <summary>
		/// If we should use rotation prediction for the hmd
		/// </summary>
		/// <value><c>true</c> if use rotation prediction; otherwise, <c>false</c>.</value>
		public bool UseRotationPrediction { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.HmdPeripheral"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="inputName">Input name.</param>
		/// <param name="positionPrediction">Position prediction.</param>
		public HmdPeripheral(string name, string inputName, IPositionPrediction positionPrediction = null) : base(name, null, positionPrediction)
		{
			Input = new ControllerInput(XDevicePlugin.GetInputDeviceHandle(inputName));
			PositionPrediction = positionPrediction;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the serial number.
		/// </summary>
		/// <returns>The serial number.</returns>
		public override string GetSerialNumber()
		{
			return XDevicePlugin.GetString(Input.handle, XDevicePlugin.kField_HMDSN8Object, "");
		}

		/// <summary>
		/// Gets the firmware version.
		/// </summary>
		/// <returns>The firmware version.</returns>
		public override string GetFirmwareVersion()
		{
			return XDevicePlugin.GetString(Input.handle, XDevicePlugin.kField_FirmwareRevisionObject, "");
		}

		/// <summary>
		/// Gets the battery level.
		/// </summary>
		/// <returns>The battery level.</returns>
		public override int GetBatteryLevel()
		{
			return Input.batteryLevel;
		}

		#endregion
	}
}