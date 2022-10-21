using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.Vision
{
	[Serializable]
	public class HeadsetSettings
	{
		#region Hardware Settings

		/// <summary>
		/// List of names this profile is fone
		/// </summary>
		public List<string> Name;

		/// <summary>
		/// The width of the physical user interface.
		/// </summary>
		[Range(0.0f, 10.0f)]
		public float UIPhysicalWidth;

		/// <summary>
		/// The height of the physical user interface.
		/// </summary>
		[Range(0.0f, 10.0f)]
		public float UIPhysicalHeight;

		/// <summary>
		/// The interpupillary distance.
		/// </summary>
		[Range(0.0f, 0.1f)]
		public float Ipd;

		/// <summary>
		/// The field of view
		/// </summary>
		[Range(25.0f, 75.0f)]
		public float Fov;

		/// <summary>
		/// The horizontal offset.
		/// </summary>
		[Range(-0.5f, 0.5f)]
		public float HorizontalOffset;

		/// <summary>
		/// The horizontal split.
		/// </summary>
		[Range(-0.5f, 0.5f)]
		public float HorizontalSplit;

		/// <summary>
		/// The vertical offset.
		/// </summary>
		[Range(-0.5f, 0.5f)]
		public float VerticalOffset;

		/// <summary>
		/// The vertical split.
		/// </summary>
		[Range(-0.5f, 0.5f)]
		public float VerticalSplit;

		/// <summary>
		/// The camera angle.
		/// </summary>
		[Range(0.0f, 50.0f)]
		public float CameraAngle;

		/// <summary>
		/// The camera offset x.
		/// </summary>
		[Range(-0.5f, 0.5f)]
		public float CameraOffsetX;

		/// <summary>
		/// The camera offset y.
		/// </summary>
		[Range(-0.5f, 0.5f)]
		public float CameraOffsetY;

		/// <summary>
		/// The camera offset z.
		/// </summary>
		[Range(-0.5f, 0.5f)]
		public float CameraOffsetZ;

		/// <summary>
		/// Gets the camera offset as a Vector3.
		/// </summary>
		/// <value>The camera offset.</value>
		public Vector3 CameraOffset
		{
			get
			{
				return new Vector3(CameraOffsetX, CameraOffsetY, CameraOffsetZ);
			}
		}

		#endregion
	}
}
