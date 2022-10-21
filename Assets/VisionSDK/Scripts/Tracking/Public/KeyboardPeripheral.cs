namespace Ximmerse.Vision
{
	public class KeyboardPeripheral : Peripheral
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.KeyboardPeripheral"/> class.
		/// </summary>
		public KeyboardPeripheral() : base("")
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the name of the device.
		/// </summary>
		/// <returns>The device name.</returns>
		public override string GetDeviceName()
		{
			return "Editor Device";
		}

		/// <summary>
		/// Gets the name of the model.
		/// </summary>
		/// <returns>The model name.</returns>
		public override string GetModelName()
		{
			return "Editor Model";
		}

		/// <summary>
		/// Gets the serial number.
		/// </summary>
		/// <returns>The serial number.</returns>
		public override string GetSerialNumber()
		{
			return "42";
		}

		/// <summary>
		/// Gets the firmware version.
		/// </summary>
		/// <returns>The firmware version.</returns>
		public override string GetFirmwareVersion()
		{
			return "1.0";
		}

		/// <summary>
		/// Gets the battery level.
		/// </summary>
		/// <returns>The battery level.</returns>
		public override int GetBatteryLevel()
		{
			return 50;
		}

		#endregion
	}
}