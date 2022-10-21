using System;
using System.Collections.Generic;

namespace Ximmerse.Vision.Internal
{
	[Serializable]
	public class Device
	{
		#region Public Properties 

		/// <summary>
		/// The device name.
		/// </summary>
		public string Name;

		/// <summary>
		/// The models.
		/// </summary>
		public List<string> Models;

		/// <summary>
		/// The base model names.
		/// </summary>
		public List<string> BaseModelNames;

		/// <summary>
		/// If the device should have the stopper in for the tray.
		/// </summary>
		public int Stopper;

		/// <summary>
		/// If the device is supported.
		/// </summary>
		public int Supported;

		/// <summary>
		/// The settings file.
		/// </summary>
		public string SettingsFile;

		/// <summary>
		/// TODO: The quality. Probably shouldn't be in the SDK
		/// </summary>
		public int Quality;

		/// <summary>
		/// The X dpi. Used for Android.
		/// </summary>
		public float XDpi;

		/// <summary>
		/// The Y dpi. Used for Android.
		/// </summary>
		public float YDpi;

		/// <summary>
		/// The width of the screen in pixels.
		/// </summary>
		public int ScreenWidth;

		/// <summary>
		/// The height of the screen in pixels.
		/// </summary>
		public int ScreenHeight;

		#endregion
	}
}