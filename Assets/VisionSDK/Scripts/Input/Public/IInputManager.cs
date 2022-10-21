using System;

namespace Ximmerse.Vision
{
	public interface IInputManager
	{
		/// <summary>
		/// An event called when a button is pressed on a peripheral, called each frame it is down.
		/// </summary>
		/// <value>A ButtonEventArgs event handler.</value>
		EventHandler<ButtonEventArgs> OnButtonPress { get; set; }

		/// <summary>
		/// An event called when a button is down on a peripheral, called once per press.
		/// </summary>
		/// <value>A ButtonEventArgs event handler.</value>
		EventHandler<ButtonEventArgs> OnButtonDown { get; set; }

		/// <summary>
		/// An event called when a button is up on a peripheral, called once per press.
		/// </summary>
		/// <value>A ButtonEventArgs event handler.</value>
		EventHandler<ButtonEventArgs> OnButtonUp { get; set; }

		/// <summary>
		/// Gets if the button is down.
		/// </summary>
		/// <returns><c>true</c>, if button down was gotten, <c>false</c> otherwise.</returns>
		/// <param name="peripheral">Peripheral.</param>
		/// <param name="button">Button.</param>
		bool GetButtonDown(Peripheral peripheral, ButtonType button);

		/// <summary>
		/// Gets if the button is pressed.
		/// </summary>
		/// <returns><c>true</c>, if button pressed was gotten, <c>false</c> otherwise.</returns>
		/// <param name="peripheral">Peripheral.</param>
		/// <param name="button">Button.</param>
		bool GetButtonPressed(Peripheral peripheral, ButtonType button);

		/// <summary>
		/// Gets if the button is up.
		/// </summary>
		/// <returns><c>true</c>, if button up was gotten, <c>false</c> otherwise.</returns>
		/// <param name="peripheral">Peripheral.</param>
		/// <param name="button">Button.</param>
		bool GetButtonUp(Peripheral peripheral, ButtonType button);

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		void Destroy();
	}
}