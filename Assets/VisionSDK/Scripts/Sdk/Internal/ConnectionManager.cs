using System;
using System.Collections.Generic;
using Ximmerse.InputSystem;
using System.Linq;

namespace Ximmerse.Vision.Internal
{
	public class ConnectionManager : IConnectionManager
	{
		#region Public Events

		/// <summary>
		/// Event for state changes for peripherals (Hmd and Saber)
		/// </summary>
		/// <value>The on peripheral state change.</value>
		public EventHandler<PeripheralStateChangeEventArgs> OnPeripheralStateChange { get; set; }

		/// <summary>
		/// List of the peripherals.
		/// </summary>
		/// <value>The peripherals.</value>
		public List<Peripheral> Peripherals { get; private set; }

		#endregion

		#region Private Properties

		private IHeartbeat heartbeat;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.Internal.ConnectionManager"/> class.
		/// </summary>
		/// <param name="heartbeat">Heartbeat.</param>
		public ConnectionManager(IHeartbeat heartbeat)
		{
			this.heartbeat = heartbeat;
			this.heartbeat.OnFrameUpdate += FrameUpdate;

			Peripherals = new List<Peripheral>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		public void Destroy()
		{
			heartbeat.OnFrameUpdate -= FrameUpdate;

			foreach (Peripheral peripheral in Peripherals)
			{
				peripheral.Destroy();
			}
		}

		/// <summary>
		/// Adds a peripheral to watch.
		/// </summary>
		/// <param name="peripheral">Peripheral.</param>
		public void AddPeripheral(Peripheral peripheral)
		{
			Peripherals.Add(peripheral);
		}

		/// <summary>
		/// Removes a peripheral to watch.
		/// </summary>
		/// <param name="peripheral">Peripheral.</param>
		public void RemovePeripheral(Peripheral peripheral)
		{
			if (Peripherals.Contains(peripheral))
			{
				Peripherals.Remove(peripheral);
			}
		}

		/// <summary>
		/// Gets the peripheral.
		/// </summary>
		/// <returns>The peripheral.</returns>
		/// <param name="name">Name.</param>
		public Peripheral GetPeripheral(string name)
		{
			return Peripherals.FirstOrDefault(item => item.Name.Equals(name));
		}

		#endregion

		#region Private Methods

		private void FrameUpdate(object sender, EventArgs eventArguments)
		{
			foreach (Peripheral peripheral in Peripherals)
			{
				if (peripheral.ControllerInput.connectionState == DeviceConnectionState.Connected && !peripheral.Connected)
				{
					peripheral.Connected = true;

					if (peripheral is ControllerPeripheral)
					{
						((ControllerPeripheral)peripheral).SetColor();
					}

					if (OnPeripheralStateChange != null)
					{
						OnPeripheralStateChange(this, new PeripheralStateChangeEventArgs(true, peripheral));
					}
				}
				else if (peripheral.ControllerInput.connectionState == DeviceConnectionState.Disconnected && peripheral.Connected)
				{
					peripheral.Connected = false;

					if (OnPeripheralStateChange != null)
					{
						OnPeripheralStateChange(this, new PeripheralStateChangeEventArgs(false, peripheral));
					}
				}
			}
		}

		#endregion
	}
}