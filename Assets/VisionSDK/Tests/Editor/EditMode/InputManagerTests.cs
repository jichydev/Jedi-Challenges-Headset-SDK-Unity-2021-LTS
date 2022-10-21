using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Collections;
using NUnit.Framework;
using Ximmerse.Vision;
using Ximmerse.Vision.Internal;

namespace Ximmerse.Vision.Tests
{
	public class InputManagerTests
	{
		[UnityTest]
		public IEnumerator InputButtonTests()
		{
			// Setup the VisionSDK.
			StereoCamera stereoCamera = (new GameObject("StereoCamera", typeof(StereoCamera))).GetComponent<StereoCamera>();
			VisionSDK visionSdk = (new GameObject("VisionSDK", typeof(VisionSDK))).GetComponent<VisionSDK>();
			Camera camera = (new GameObject("Camera", typeof(Camera))).GetComponent<Camera>();
			Heartbeat heartbeat = camera.gameObject.AddComponent<Heartbeat>();

			try
			{
				// Hook up the cameras.
				stereoCamera.LeftCamera = stereoCamera.RightCamera = stereoCamera.BottomCamera = stereoCamera.SingleCamera = camera;

				// Init the sdk.
				visionSdk.Heartbeat = heartbeat;
				visionSdk.StereoCamera = stereoCamera;
				visionSdk.Init(false);

				// Tracker a controller.
				ControllerPeripheral controller = new ControllerPeripheral("");
				visionSdk.Connections.AddPeripheral(controller);

				// No button is being pressed. Not sure how to fake it with the ximmerse sdk.
				Assert.AreEqual(visionSdk.Input.GetButtonDown(controller, ButtonType.SaberActivate), false);
				Assert.AreEqual(visionSdk.Input.GetButtonPressed(controller, ButtonType.SaberActivate), false);
				Assert.AreEqual(visionSdk.Input.GetButtonUp(controller, ButtonType.SaberActivate), false);
			}
			catch (Exception error)
			{
				Assert.Fail(error.Message);
			}
			finally
			{
				Ximmerse.InputSystem.XDevicePlugin.Exit();

				GameObject.DestroyImmediate(stereoCamera.gameObject);
				GameObject.DestroyImmediate(camera.gameObject);
				GameObject.DestroyImmediate(visionSdk.gameObject);
			}
			yield return null;
		}
	}
}