using System;
using UnityEngine;
using Ximmerse.InputSystem;

namespace Ximmerse.Vision.Internal
{
	public class InputManager : IInputManager
	{
		#region Public Events

		/// <summary>
		/// An event called when a button is pressed on a peripheral, called each frame it is down.
		/// </summary>
		/// <value>A ButtonEventArgs event handler.</value>
		public EventHandler<ButtonEventArgs> OnButtonPress { get; set; }

		/// <summary>
		/// An event called when a button is down on a peripheral, called once per press.
		/// </summary>
		/// <value>A ButtonEventArgs event handler.</value>
		public EventHandler<ButtonEventArgs> OnButtonDown { get; set; }

		/// <summary>
		/// An event called when a button is up on a peripheral, called once per press.
		/// </summary>
		/// <value>A ButtonEventArgs event handler.</value>
		public EventHandler<ButtonEventArgs> OnButtonUp { get; set; }

		#endregion

		#region Private Properties

		private IHeartbeat heartbeat;
		private readonly IConnectionManager connections;
		private readonly ButtonType[] inputTypes = (ButtonType[])Enum.GetValues(typeof(ButtonType));

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.Internal.InputManager"/> class.
		/// </summary>
		/// <param name="heartbeat">The heartbeat instance to use.</param>
		/// <param name = "connections"></param>
		public InputManager(IHeartbeat heartbeat, IConnectionManager connections)
		{
			this.heartbeat = heartbeat;
			this.connections = connections;

			this.heartbeat.OnFrameUpdate += Update;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets if the button is down
		/// </summary>
		/// <returns><c>true</c>, if button down was gotten, <c>false</c> otherwise.</returns>
		/// <param name="peripheral">Peripheral.</param>
		/// <param name="button">Button.</param>
		public bool GetButtonDown(Peripheral peripheral, ButtonType button)
		{
			ControllerInput input = GetControllerInput(peripheral);
			return (input != null) && input.GetButtonDown((uint)button);
		}

		/// <summary>
		/// Gets if the button is pressed.
		/// </summary>
		/// <returns><c>true</c>, if button pressed was gotten, <c>false</c> otherwise.</returns>
		/// <param name="peripheral">Peripheral.</param>
		/// <param name="button">Button.</param>
		public bool GetButtonPressed(Peripheral peripheral, ButtonType button)
		{
			ControllerInput input = GetControllerInput(peripheral);
			return (input != null) && input.GetButton((uint)button);
		}

		/// <summary>
		/// Gets if the button is up.
		/// </summary>
		/// <returns><c>true</c>, if button up was gotten, <c>false</c> otherwise.</returns>
		/// <param name="peripheral">Peripheral.</param>
		/// <param name="button">Button.</param>
		public bool GetButtonUp(Peripheral peripheral, ButtonType button)
		{
			ControllerInput input = GetControllerInput(peripheral);
			return (input != null) && input.GetButtonUp((uint)button);
		}

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		public void Destroy()
		{
			// Stop Listening
			heartbeat.OnFrameUpdate -= Update;

			// Clear Events
			OnButtonPress = null;
			OnButtonDown = null;
			OnButtonUp = null;
		}

		#endregion

		#region Private Methods

		private void Update(object sender, EventArgs arguments)
		{
			for (int i = 0; i < connections.Peripherals.Count; ++i)
			{
				CheckControllerButtons(connections.Peripherals[i]);
			}

			#if UNITY_EDITOR

			CheckKeyboardInput();

			#endif
		}

		#if UNITY_EDITOR

		private KeyCode[] keyboardInput = { KeyCode.M, KeyCode.Backspace, KeyCode.Return, KeyCode.Mouse0, KeyCode.Mouse1,KeyCode.Mouse2,KeyCode.Space, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2 };
		private ButtonType[] visionInput = { ButtonType.HmdMenu, ButtonType.HmdBack, ButtonType.HmdSelect, ButtonType.SaberControl, ButtonType.SaberActivate, ButtonType.Trigger, ButtonType.Touchpad, ButtonType.App, ButtonType.Home};
		private KeyboardPeripheral keyboard = new KeyboardPeripheral();

		private void CheckKeyboardInput()
		{
			// checking for each valid keyboard input type
			foreach (KeyCode code in keyboardInput)
			{
				// getting index
				ButtonType inputType = visionInput[Array.IndexOf(keyboardInput, code)];

				// checking for key down event
				if (Input.GetKeyDown(code))
				{
					// dispatching the button down event
					if (OnButtonDown != null)
					{
						OnButtonDown(this, new ButtonEventArgs(keyboard, inputType, ButtonEventType.Down));
					}
				}

				// checking for key press event
				if (Input.GetKey(code))
				{
					// dispatching the button press event
					if (OnButtonPress != null)
					{
						OnButtonPress(this, new ButtonEventArgs(keyboard, inputType, ButtonEventType.Press));
					}
				}

				// checking for key up event
				if (Input.GetKeyUp(code))
				{
					// dispatching the button up event
					if (OnButtonUp != null)
					{
						OnButtonUp(this, new ButtonEventArgs(keyboard, inputType, ButtonEventType.Up));
					}
				}
			}
		}

		#endif

		private void CheckControllerButtons(Peripheral peripheral)
		{

			foreach (ButtonType value in inputTypes)
			{
				if (GetButtonDown(peripheral, value))
				{
					if (OnButtonDown != null)
					{
						OnButtonDown(this, new ButtonEventArgs(peripheral, value, ButtonEventType.Down));
					}
				}

				if (GetButtonPressed(peripheral, value))
				{
					if (OnButtonPress != null)
					{
						OnButtonPress(this, new ButtonEventArgs(peripheral, value, ButtonEventType.Press));
					}
				}

				if (GetButtonUp(peripheral, value))
				{
					if (OnButtonUp != null)
					{
						OnButtonUp(this, new ButtonEventArgs(peripheral, value, ButtonEventType.Up));
					}
				}
			}
		}

		private static ControllerInput GetControllerInput(Peripheral peripheral)
		{
			ControllerInput input = peripheral.ControllerInput;

			if (peripheral is HmdPeripheral)
			{
				input = ((HmdPeripheral)peripheral).Input;
			}

            return input;
		}

		#endregion
	}
}