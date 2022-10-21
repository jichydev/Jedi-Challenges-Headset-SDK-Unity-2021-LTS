using System;

namespace Ximmerse.Vision
{
	public interface ISettingsManager
	{
		/// <summary>
		/// Event for when the Headset or Device Settings being used are changed.
		/// </summary>
		/// <value>The on settings changed.</value>
		EventHandler<SettingsChangeEventArgs> OnSettingsChanged { get; set; }

		/// <summary>
		/// Gets or sets the headset settings.
		/// </summary>
		/// <value>The headset settings.</value>
		HeadsetSettings HeadsetSettings { get; set; }

		/// <summary>
		/// Gets or sets the device settings.
		/// </summary>
		/// <value>The device settings.</value>
		DeviceSettings DeviceSettings { get; set; }

		/// <summary>
		/// Gets the recomended settings for device.
		/// </summary>
		/// <returns>The recomended settings for device.</returns>
		/// <param name="deviceName">Device name.</param>
		DeviceSettings GetRecomendedSettingsForDevice(string deviceName = null);

		/// <summary>
		/// Gets the recomended settings for a headset.
		/// </summary>
		/// <returns>The recomended settings for a headset.</returns>
		HeadsetSettings GetRecomendedSettingsForHeadset();

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		void Destroy();
	}
}