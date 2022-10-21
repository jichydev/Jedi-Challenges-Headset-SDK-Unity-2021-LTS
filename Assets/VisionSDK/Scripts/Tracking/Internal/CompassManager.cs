using System.Runtime.InteropServices;
using UnityEngine;

namespace Ximmerse.Vision.Internal
{
	public class CompassManager
	{
		#if UNITY_IPHONE

		[DllImport ("__Internal")]
		private static extern void LocationManagerInit();
		[DllImport ("__Internal")]
		private static extern float LocationManagerGetMagneticHeading();
		[DllImport ("__Internal")]
		private static extern float LocationManagerGetMagneticHeadingRawX();
		[DllImport ("__Internal")]
		private static extern float LocationManagerGetMagneticHeadingRawY();
		[DllImport ("__Internal")]
		private static extern float LocationManagerGetMagneticHeadingRawZ();

		#endif

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Ximmerse.Vision.Internal.CompassManager"/> class.
		/// </summary>
		public CompassManager()
		{
			#if UNITY_IOS && !UNITY_EDITOR
			LocationManagerInit();
			#endif
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the magnetic north heading.
		/// </summary>
		/// <returns>The heading.</returns>
		public Vector3 GetHeading()
		{
			#if UNITY_ANDROID
			return Input.compass.rawVector;
			#elif UNITY_IOS && !UNITY_EDITOR
			Vector3 result;
			result.x = LocationManagerGetMagneticHeadingRawY();
			result.y = -LocationManagerGetMagneticHeadingRawX();
			result.z = LocationManagerGetMagneticHeadingRawZ();

			if (result.x < 0.1f && result.y < 0.1f && result.z < 0.1f)
			{
				result = Input.compass.rawVector;
			}

			return result;
			#else
			return Input.compass.rawVector;
			#endif
		}

		/// <summary>
		/// Gets the target correction from the gyro to the compass.
		/// </summary>
		/// <returns>The target correction.</returns>
		/// <param name="gyroOrientation">Gyro orientation.</param>
		public Quaternion GetTargetCorrection(Quaternion gyroOrientation)
		{
			Vector3 gravity = Input.gyro.gravity.normalized;
			Vector3 rawVector = GetHeading();
			Vector3 flatNorth = rawVector - Vector3.Dot(gravity, rawVector) * gravity;
			Quaternion co = Quaternion.Inverse(Quaternion.LookRotation(flatNorth, -gravity));
			Quaternion compassOrientation = co;
			compassOrientation.x = -co.x;
			compassOrientation.y = co.z;  // swap y and z
			compassOrientation.z = co.y;
			compassOrientation = Quaternion.Euler (90f, 0f, 0f) * compassOrientation;

			// Calculate the target correction factor
			return compassOrientation * Quaternion.Inverse(gyroOrientation);
		}

		#endregion
	}
}