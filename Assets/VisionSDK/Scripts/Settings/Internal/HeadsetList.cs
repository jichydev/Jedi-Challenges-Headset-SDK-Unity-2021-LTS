using System;
using System.Collections.Generic;

namespace Ximmerse.Vision.Internal
{
	[Serializable]
	public class HeadsetList
	{
		#region Public Properties

		/// <summary>
		/// The version.
		/// </summary>
		public int Version;

		/// <summary>
		/// The list of headsets.
		/// </summary>
		public List<HeadsetSettings> Headsets;

		#endregion
	}
}