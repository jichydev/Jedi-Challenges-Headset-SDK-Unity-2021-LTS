using System;
using UnityEngine;

namespace Ximmerse.Vision
{
	public class BeaconStateChangeEventArgs : EventArgs
	{
		#region Public Properties

		/// <summary>
		/// If the beacon is currently tracked.
		/// </summary>
		public bool Tracked;

		/// <summary>
		/// Either the current position or the last known position (if not Tracked).
		/// </summary>
		public Vector3 Position;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.BeaconStateChangeEventArgs"/> class.
		/// </summary>
		/// <param name="tracked">If set to <c>true</c> tracked.</param>
		/// <param name="position">Position.</param>
		public BeaconStateChangeEventArgs(bool tracked, Vector3 position)
		{
			Tracked = tracked;
			Position = position;
		}

		#endregion
	}
}