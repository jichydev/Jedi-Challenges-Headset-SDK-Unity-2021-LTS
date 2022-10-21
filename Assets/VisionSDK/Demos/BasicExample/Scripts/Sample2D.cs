using UnityEngine;
using UnityEngine.SceneManagement;
using Ximmerse.Vision;
using System;
using UnityEngine.UI;
using Ximmerse.InputSystem;
using System.Collections;

namespace Ximmerse.Example
{
    public class Sample2D : MonoBehaviour
    {
        #region Public Properties

        /// <summary>
        /// The Hmd watcher.
        /// </summary>
        public VisionSDK Sdk;

        /// <summary>
        /// The start button.
        /// </summary>
        public Image StartDemoImage;

        /// <summary>
        /// The pair button.
        /// </summary>
        public Button PairButton;

        /// <summary>
        /// The calibrate button.
        /// </summary>
        public Button CalibrateButton;

        #endregion

        #region Private Properties

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
            Sdk.Init(true);

            // Add a controller
            controller = new ControllerPeripheral("XCobra-0");
            Sdk.Connections.AddPeripheral(controller);

            // Listeners
            Sdk.Connections.OnPeripheralStateChange += OnPeripheralStateChange;
        }

        private void OnDestroy()
        {
            Sdk.Connections.OnPeripheralStateChange -= OnPeripheralStateChange;
        }

#endregion

#region Public Methods

        public void StartDemo()
        {
            SceneManager.LoadScene("BasicExampleSceneTwo");
        }

        public void StartDeviceSettings()
        {
            SceneManager.LoadScene("DeviceSettingsExample");
        }

        public void StartHeadsetSettings()
        {
            SceneManager.LoadScene("HeadsetSettingsExample");
        }

        public void StartPairing()
        {
            // Start scanning for peripherals
            Sdk.Tracking.StartPairing();

            // Update text
            PairButton.GetComponentInChildren<Text>().text = "Looking...";
            PairButton.GetComponent<Image>().color = Color.blue;

            // Stop scanning in 10 seconds
            Invoke("StopPairing", 10.0f);
        }

        public void StopPairing()
        {
            // Stop scanning
            Sdk.Tracking.StopPairing();

            // Update text
            PairButton.GetComponent<Image>().color = Color.white;
            PairButton.GetComponentInChildren<Text>().text = "Start Pairing";
        }

        public void StartCalibration()
        {
            // Can't calibrate if nothing is paired.
            if (!controller.Connected)
            {
                return;
            }

            // Start the calibration
            controller.StartCalibration(CalibrationStateChanged);

            // Update the color and text.
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibrating...";
            CalibrateButton.GetComponent<Image>().color = Color.blue;

            // Force stop after 30 if not completed.
            Invoke("StopCalibration", 30.0f);
        }

        public void StopCalibration()
        {
            // Stop the calibration
            controller.StopCalibrating();

            // Update the color and text.
            CalibrateButton.GetComponent<Image>().color = Color.white;
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibrate Saber";
        }

#endregion

#region Private Methods
        private void OnPeripheralStateChange(object sender, PeripheralStateChangeEventArgs eventArguments)
        {
            Debug.Log("OnPeripheralStateChange: " + eventArguments.Peripheral + " " + eventArguments.Connected);

            // Peripheral Type
            if (eventArguments.Peripheral is HmdPeripheral)
            {
                // Update the Start Demo Button color
                StartDemoImage.color = (eventArguments.Connected) ? Color.green : Color.white;
            }
            else
            {
                if (eventArguments.Connected)
                {
                    // Stop Pairing now that it is found.
                    CancelInvoke("StopPairing");
                    StopPairing();

                    // Update the pair button color
                    PairButton.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    // Update the pair button color
                    PairButton.GetComponent<Image>().color = Color.white;
                }
            }
        }

        private void CalibrationStateChanged(CalibrationState state)
        {
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibration State: " + state;

            switch (state)
            {
                case CalibrationState.Complete:
                    // Calibration is done, stop it.
                    CancelInvoke("StopCalibration");
                    StopCalibration();
                    break;
            }
        }
#endregion
    }
}