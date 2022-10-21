using System;
using System.Collections.Generic;

namespace Ximmerse.Vision.Internal
{
	[Serializable]
	public class Devices
	{
		#region Public Properties

		/// <summary>
		/// The version.
		/// </summary>
		public int Version;

		/// <summary>
		/// The list of platforms.
		/// </summary>
		public List<DevicePlatform> Platforms;

		#endregion
	}
}