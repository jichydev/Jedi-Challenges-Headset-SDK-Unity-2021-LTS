using System;

namespace Ximmerse.Vision
{
	/// <summary>
	/// Button event type.
	/// </summary>
	public enum ButtonEventType
	{
		Down,
		Press,
		Up
	}

	public class ButtonEventArgs : EventArgs
	{
		#region Public Properties

		/// <summary>
		/// The button that was pressed.
		/// </summary>
		public ButtonType Button;

		/// <summary>
		/// The peripheral that had the button that was pressed.
		/// </summary>
		public Peripheral Peripheral;

		/// <summary>
		/// The type of button event that was dispatched
		/// </summary>
		public ButtonEventType Type;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.ButtonEventArgs"/> class.
		/// </summary>
		/// <param name="peripheral">The peripheral that had the button that was pressed.</param>
		/// <param name="button">The button that was pressed.</param>
		/// <param name="type">The button event type that was dispatched.</param>
		public ButtonEventArgs(Peripheral peripheral, ButtonType button, ButtonEventType type)
		{
			Peripheral = peripheral;
			Button = button;
			Type = type;
		}

		#endregion
	}
}