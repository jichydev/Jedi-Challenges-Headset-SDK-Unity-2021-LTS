using Ximmerse.Vision;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ximmerse.InputSystem;
using System.Collections;
//using LenovoMirageARSDK;

namespace Ximmerse.Example
{
    public class Sample3D : MonoBehaviour
    {
        #region Public Properties

        // The Red controllers transform.
        public Transform Controller;

        // Scene to load when you press the trigger button.
        public string NextScene;

        // The Vision SDK
        public VisionSDK Sdk;

        // A Sample Logger
        public SampleLogger Logger;

        #endregion

        #region Private Properties

        // Saber color
        private static int saberColor = (int)ColorID.BLUE;

        private ControllerPeripheral controller;

        #endregion

        #region Unity Methods

        //@EDIT:
        private void Awake()
        {
        }

        //@EDIT:
        private void Start()
        {
            /*
			1st parameter: Single Camera vs Stereo Camera.
			2nd parameter: IVisionLogger, if not provided it will use VisionLogger which just suppresses logs.
			3rd parameter: IDeviceManager, if not provided it will use DeviceManager, which only supports loading logs built into the app.
			*/
            Sdk.Init(false, Logger);

            Sdk.Logger.Log("SDK Setup");

            Sdk.Logger.Log("SDK Version：" + VisionSDK.Version);

            Sdk.Logger.Log("Ximmerse Android Libray Version：" + VisionSDK.XimmerseAndroidLibraryVersion);

            Sdk.Logger.Log("Ximmerse iOS Libray Version：" + VisionSDK.XimmerseiOSLibraryVersion);

            // Enable Mag Correction, it defaults off.
            Sdk.StereoCamera.UseMagnetometerCorrection = false;

            // If you want to use prediction for the HMD (Phone).
            Sdk.Tracking.Hmd.UsePositionPrediction = false;
            Sdk.Tracking.Hmd.UseRotationPrediction = false;

            // Setup Controllers with a Transform if one
            controller = new ControllerPeripheral("XCobra-0", Controller, null, (ColorID)saberColor);
            controller.UsePositionPrediction = true;

            Sdk.Connections.AddPeripheral(controller);

            // Connection Events
            Sdk.Connections.OnPeripheralStateChange += OnPeripheralStateChange;

            // Input Events
            Sdk.Input.OnButtonPress += OnButtonPressed;
            Sdk.Input.OnButtonDown += OnButtonDown;
            Sdk.Input.OnButtonUp += OnButtonUp;

            // Tracking Events
            Sdk.Tracking.OnBeaconStateChange += OnBeaconStateChange;
        }

        private void OnDestroy()
        {
            // Remove Events
            Sdk.Connections.OnPeripheralStateChange -= OnPeripheralStateChange;
            Sdk.Input.OnButtonPress -= OnButtonPressed;
            Sdk.Input.OnButtonDown -= OnButtonDown;
            Sdk.Input.OnButtonUp -= OnButtonUp;
            Sdk.Tracking.OnBeaconStateChange -= OnBeaconStateChange;
        }

        #endregion

        #region Private Methods

        private void OnPeripheralStateChange(object sender, PeripheralStateChangeEventArgs eventArguments)
        {
            Sdk.Logger.Log("Is " + eventArguments.Peripheral.Name + " Connected? " + eventArguments.Connected);

            if (eventArguments.Peripheral is ControllerPeripheral && eventArguments.Connected)
            {
                CancelInvoke("StopPairing");
                StopPairing();

                if (eventArguments.Peripheral is ControllerPeripheral)
                {
                    ((ControllerPeripheral)eventArguments.Peripheral).SetColor(saberColor);
                }
            }
        }

        private void OnButtonDown(object sender, ButtonEventArgs eventArguments)
        {
            Sdk.Logger.Log(eventArguments.Peripheral.Name + " (D): " + eventArguments.Button);

            // Change Scenes
            if (eventArguments.Button == Vision.ButtonType.HmdSelect)
            {
                GotoScene(NextScene);
            }

            // Recenter the player and Saber
            if (eventArguments.Button == Vision.ButtonType.SaberControl)
            {
                Sdk.Tracking.Recenter(true);
            }

            // Start Pairing
            if (eventArguments.Button == Vision.ButtonType.HmdBack)
            {
                Sdk.Logger.Log("Looking for peripheral");
                Sdk.Tracking.StartPairing();
                Invoke("StopPairing", 10.0f);
            }
        }

        private void StopPairing()
        {
            Sdk.Logger.Log("Stop looking for peripheral");
            Sdk.Tracking.StopPairing();
        }

        private void OnButtonUp(object sender, ButtonEventArgs eventArguments)
        {
            Sdk.Logger.Log(eventArguments.Peripheral.Name + " (U): " + eventArguments.Button);

            Sdk.Logger.Log("Peripheral Device Name: " + eventArguments.Peripheral.GetDeviceName());
            Sdk.Logger.Log("Peripheral FW Info: " + eventArguments.Peripheral.GetFirmwareVersion());
            Sdk.Logger.Log("Peripheral Model Name: " + eventArguments.Peripheral.GetModelName());
            Sdk.Logger.Log("Peripheral Serial Number: " + eventArguments.Peripheral.GetSerialNumber());
            Sdk.Logger.Log("Peripheral Battery: " + eventArguments.Peripheral.GetBatteryLevel());
        }

        private void OnButtonPressed(object sender, ButtonEventArgs eventArguments)
        {
            Sdk.Logger.Log(eventArguments.Peripheral.Name + " (P): " + eventArguments.Button);
        }

        private void OnBeaconStateChange(object sender, BeaconStateChangeEventArgs eventArguments)
        {
            Sdk.Logger.Log("Beacon Tracked: " + eventArguments.Tracked + " - Position: " + eventArguments.Position);

            Sdk.Logger.Log("HMD Device Name: " + Sdk.Tracking.Hmd.GetDeviceName());
            Sdk.Logger.Log("HMD FW Info: " + Sdk.Tracking.Hmd.GetFirmwareVersion());
            Sdk.Logger.Log("HMD Modle Name: " + Sdk.Tracking.Hmd.GetModelName());
            Sdk.Logger.Log("HMD Serial Number: " + Sdk.Tracking.Hmd.GetSerialNumber());
            Sdk.Logger.Log("HMD Battery: " + Sdk.Tracking.Hmd.GetBatteryLevel());
        }

        private void GotoScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private void Update()
        {
            //@EDIT:
            if (!VisionSDK.Instance.Inited) return;

            // If there is a touchpad button
            if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.PrimaryThumbMove))
            {
                Sdk.Logger.Log("touchpad = " + controller.ControllerInput.touchPos.ToString("F2"));

                if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.PrimaryThumbUp))
                {
                    Sdk.Logger.Log("Button = " + "PrimaryThumbUp");
                }

                if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.PrimaryThumbDown))
                {
                    Sdk.Logger.Log("Button = " + "PrimaryThumbDown");
                }

                if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.PrimaryThumbLeft))
                {
                    Sdk.Logger.Log("Button = " + "PrimaryThumbLeft");
                }

                if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.PrimaryThumbRight))
                {
                    Sdk.Logger.Log("Button = " + "PrimaryThumbRight");
                }
            }

            //Trigger
            if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.PrimaryTrigger))
            {
                Sdk.Logger.Log("Button = " + "PrimaryTrigger");
            }

            // The keys below the touchpad
            if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.PrimaryThumb))
            {
                Sdk.Logger.Log("Button = " + "PrimaryThumb");
            }

            if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.PrimaryShoulder))
            {
                Sdk.Logger.Log("Button = " + "PrimaryShoulder");
            }

            if (controller.ControllerInput.GetButtonDown(Ximmerse.InputSystem.ControllerButton.SecondaryShoulder))
            {
                Sdk.Logger.Log("Button = " + "SecondaryShoulder");
            }

        }
        #endregion
    }
}