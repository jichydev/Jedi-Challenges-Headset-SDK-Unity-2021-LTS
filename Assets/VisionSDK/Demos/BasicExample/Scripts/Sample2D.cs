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

        private ControllerPeripheral controller1;
        private ControllerPeripheral controller2;

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
            controller1 = new ControllerPeripheral("XCobra-0", null, null, ColorID.BLUE);
            controller2 = new ControllerPeripheral("XCobra-1", null, null, ColorID.RED);

            Sdk.Connections.AddPeripheral(controller1);
            Sdk.Connections.AddPeripheral(controller2);

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

        public void StartCalibration1()
        {
            // Can't calibrate if nothing is paired.
            if (!controller1.Connected)
            {
                return;
            }

            // Start the calibration
            controller1.StartCalibration(CalibrationStateChanged1);

            // Update the color and text.
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibrating...";
            CalibrateButton.GetComponent<Image>().color = Color.blue;

            // Force stop after 30 if not completed.
            Invoke("StopCalibration1", 30.0f);
        }

        public void StartCalibration2()
        {
            // Can't calibrate if nothing is paired.
            if (!controller2.Connected)
            {
                return;
            }

            // Start the calibration
            controller2.StartCalibration(CalibrationStateChanged2);

            // Update the color and text.
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibrating...";
            CalibrateButton.GetComponent<Image>().color = Color.blue;

            // Force stop after 30 if not completed.
            Invoke("StopCalibration2", 30.0f);
        }

        public void StopCalibration1()
        {
            // Stop the calibration
            controller1.StopCalibrating();

            // Update the color and text.
            CalibrateButton.GetComponent<Image>().color = Color.white;
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibrate controller 1";
        }

        public void StopCalibration2()
        {
            // Stop the calibration
            controller2.StopCalibrating();

            // Update the color and text.
            CalibrateButton.GetComponent<Image>().color = Color.white;
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibrate controller 2";
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
                    //CancelInvoke("StopPairing");
                    //StopPairing();

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

        private void CalibrationStateChanged1(CalibrationState state)
        {
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibration State: " + state;

            switch (state)
            {
                case CalibrationState.Complete:
                    // Calibration is done, stop it.
                    CancelInvoke("StopCalibration1");
                    StopCalibration1();
                    break;
            }
        }

        private void CalibrationStateChanged2(CalibrationState state)
        {
            CalibrateButton.GetComponentInChildren<Text>().text = "Calibration State: " + state;

            switch (state)
            {
                case CalibrationState.Complete:
                    // Calibration is done, stop it.
                    CancelInvoke("StopCalibration2");
                    StopCalibration2();
                    break;
            }
        }
        #endregion
    }
}