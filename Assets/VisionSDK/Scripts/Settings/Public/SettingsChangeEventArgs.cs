using System;

namespace Ximmerse.Vision
{
	/// <summary>
	/// Device settings change event arguments. We don't want to pass the settings as they should get them
	/// from the DeviceManager, which is the Sender of this event.
	/// </summary>
	public class SettingsChangeEventArgs : EventArgs
	{
		#region Public Properties

		/// <summary>
		/// The setting type.
		/// </summary>
		public SettingType Type;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.SettingsChangeEventArgs"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		public SettingsChangeEventArgs(SettingType type)
		{
			Type = type;
		}

		#endregion
	}
}