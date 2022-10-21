using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.Vision
{
	public interface IXimmerseTracker
	{
		/// <summary>
		/// Listener for when the beacon gains or loses tracking.
		/// </summary>
		/// <value>The on beacon state change event.</value>
		EventHandler<BeaconStateChangeEventArgs> OnBeaconStateChange { get; set; }

		/// <summary>
		/// If the beacon is currently being tracked
		/// </summary>
		/// <value><c>true</c> if this instance is tracked; otherwise, <c>false</c>.</value>
		bool IsBeaconTracked { get; }

		/// <summary>
		/// The Hmd.
		/// </summary>
		HmdPeripheral Hmd { get; }

		/// <summary>
		/// Re-centers the forward vector for the player the next frame the beacon is seen.
		/// </summary>
		/// <param name="peripheralsToo">Whether to recenter all attached peripherals as well.</param>
		void Recenter(bool peripheralsToo = false);

		/// <summary>
		/// This should be called when the tracker instance is no longer needed.
		/// </summary>
		void Destroy();

		/// <summary>
		/// Start this tracking.
		/// </summary>
		void StartTracking();

		/// <summary>
		/// Stop this tracking.
		/// </summary>
		void StopTracking();

		/// <summary>
		/// Pauses the tracking.
		/// </summary>
		void PauseTracking();

		/// <summary>
		/// Resume the tracking.
		/// </summary>
		void ResumeTracking();

		/// <summary>
		/// Starts the pairing.
		/// </summary>
		void StartPairing();

		/// <summary>
		/// Stops the pairing.
		/// </summary>
		void StopPairing();

		/// <summary>
		/// Get the position of a given blob color with respect to the HMD.
		/// </summary>
		/// <returns>The position of the blob color</returns>
		/// <param name="color">The color of the blob to return the position of.</param>
		Vector3 GetBlobPosition(int color);
	}
}