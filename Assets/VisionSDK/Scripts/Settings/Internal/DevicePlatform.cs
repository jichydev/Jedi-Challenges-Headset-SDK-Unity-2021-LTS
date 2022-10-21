using System;
using System.Collections.Generic;

namespace Ximmerse.Vision.Internal
{
	[Serializable]
	public class DevicePlatform
	{
		#region Public Properties

		/// <summary>
		/// The platform.
		/// </summary>
		public string Platform;

		/// <summary>
		/// The list of devices.
		/// </summary>
		public List<Device> Devices;

		#endregion
	}
}