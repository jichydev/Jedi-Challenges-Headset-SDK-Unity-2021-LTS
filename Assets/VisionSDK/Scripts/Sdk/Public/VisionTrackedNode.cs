using UnityEngine;
using UnityEngine.XR;

namespace Ximmerse.Vision
{
	/// <summary>
	/// Vision node type.
	/// </summary>
	public enum VisionNode
	{
		Controller = XRNode.GameController,
		Head = XRNode.Head,
		Beacon = XRNode.TrackingReference
	}

	public class VisionTrackedNode : MonoBehaviour
	{
		/// <summary>
		/// The type of the node.
		/// </summary>
		public VisionNode NodeType;

		// TODO: Is there a way around this? We techincally support multiple peripherals so we need something to know which...
		/// <summary>
		/// The name of the peripheral.
		/// </summary>
		[Header("Only used with GameController type")]
		public string PeripheralName;
	}
}