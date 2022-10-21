using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Collections;
using NUnit.Framework;
using Ximmerse.Vision;
using Ximmerse.Vision.Internal;

namespace Ximmerse.Vision.Tests
{	
	public class TrackedNodesTest
	{
		[UnityTestAttribute]
		public IEnumerator MovingTrackedNode()
		{
			// Setup the VisionSDK.
			StereoCamera stereoCamera = (new GameObject("StereoCamera", typeof(StereoCamera))).GetComponent<StereoCamera>();
			VisionSDK visionSdk = (new GameObject("VisionSDK", typeof(VisionSDK))).GetComponent<VisionSDK>();
			Camera camera = (new GameObject("Camera", typeof(Camera))).GetComponent<Camera>();
			Heartbeat heartbeat = camera.gameObject.AddComponent<Heartbeat>();

			try
			{
				// Tracked Node
				VisionTrackedNode node = (new GameObject("TrackedNode", typeof(VisionTrackedNode))).GetComponent<VisionTrackedNode>();
				node.NodeType = VisionNode.Head;

				// Hook up the cameras.
				stereoCamera.LeftCamera = stereoCamera.RightCamera = stereoCamera.BottomCamera = stereoCamera.SingleCamera = camera;

				// Linkages.
				visionSdk.Heartbeat = heartbeat;
				visionSdk.StereoCamera = stereoCamera;

				// Add Node
				visionSdk.TrackedNodes.Add(node);

				// Init
				visionSdk.Init(false);

				// Move the player.
				stereoCamera.transform.localPosition = new Vector3(2, 1, 6);
				stereoCamera.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 180));

				// Wait a frame
				// TODO: I should be able to do yield return null; but it isn't working...
				heartbeat.OnBeforeRender.Invoke(this, null);

				// No button is being pressed. Not sure how to fake it with the ximmerse sdk.
				Assert.AreEqual(node.transform.localPosition, stereoCamera.transform.localPosition);
				Assert.AreEqual(node.transform.localRotation, stereoCamera.transform.rotation);
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