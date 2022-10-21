using System;
using System.Collections.Generic;

namespace Ximmerse.Vision
{
	public interface IConnectionManager
	{
		/// <summary>
		/// Event for state changes for peripherals (Hmd and Saber)
		/// </summary>
		/// <value>The on peripheral state change.</value>
		EventHandler<PeripheralStateChangeEventArgs> OnPeripheralStateChange { get; set; }

		/// <summary>
		/// Gets or sets the peripherals.
		/// </summary>
		/// <value>The peripherals.</value>
		List<Peripheral> Peripherals { get; }

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		void Destroy();

		/// <summary>
		/// Adds a peripheral to watch.
		/// </summary>
		/// <param name="peripheral">Peripheral.</param>
		void AddPeripheral(Peripheral peripheral);

		/// <summary>
		/// Removes a peripheral to watch.
		/// </summary>
		/// <param name="peripheral">Peripheral.</param>
		void RemovePeripheral(Peripheral peripheral);

		/// <summary>
		/// Gets the peripheral.
		/// </summary>
		/// <returns>The peripheral.</returns>
		/// <param name="name">Name.</param>
		Peripheral GetPeripheral(string name);
	}
}