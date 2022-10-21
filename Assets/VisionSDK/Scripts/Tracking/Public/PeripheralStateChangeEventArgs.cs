using System;

namespace Ximmerse.Vision
{
	public class PeripheralStateChangeEventArgs : EventArgs
	{
		#region Public Properties

		/// <summary>
		/// If the peripheral was connected or disconnected.
		/// </summary>
		public bool Connected;

		/// <summary>
		/// The peripheral that was connected or disconnected.
		/// </summary>
		public Peripheral Peripheral;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.PeripheralStateChangeEventArgs"/> class.
		/// </summary>
		/// <param name="connected">If set to <c>true</c> connected.</param>
		/// <param name="peripheral">The peripheral that was connected or disconnected.</param>
		public PeripheralStateChangeEventArgs(bool connected, Peripheral peripheral)
		{
			Connected = connected;
			Peripheral = peripheral;
		}

		#endregion
	}
}