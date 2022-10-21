using UnityEngine;

namespace Ximmerse.Vision.Internal
{
	public class RotationPrediction
	{
		#region Private Properties

		// List of the last rotations, used for prediction
		private Quaternion[] pastFrameRotations = new Quaternion[10];

		// Current index of the past rotations
		private int pastRotationIndex = 0;

		// Offsets for the players head
		private const float RotationYPoint = 0.10f;
		private const float RotationZPoint = 0.10f;

		// Lerping
		private const float YRotationThresholdForDelay = 0.4f;
		private const float TimeToZero = 0.5f;
		private const float TimeToOne = 0.25f;

		// Ramp to full prediction
		private float percentageOfDelayToUse = 0;
		private float lastFrameYRot = 0;
		private float lastFrameXRot = 0;
		private int lastFrameCount = 0;

		#endregion

		#region Public Methods

		/// <summary>
		/// Updates the prediction based on the new rotation.
		/// </summary>
		/// <param name="rotation">Rotation.</param>
		public void Update(Quaternion rotation)
		{
			float rotDifY1 = Mathf.Abs(lastFrameYRot - rotation.eulerAngles.y);
			float rotDifY2 = Mathf.Abs(lastFrameYRot - (rotation.eulerAngles.y + 360));
			float rotDifY3 = Mathf.Abs(lastFrameYRot - (rotation.eulerAngles.y - 360));

			float rotDifX1 = Mathf.Abs(lastFrameXRot - rotation.eulerAngles.x);
			float rotDifX2 = Mathf.Abs(lastFrameXRot - (rotation.eulerAngles.x + 360));
			float rotDifX3 = Mathf.Abs(lastFrameXRot - (rotation.eulerAngles.x - 360));

			float rotDifY = Mathf.Min(rotDifY3, Mathf.Min(rotDifY2, rotDifY1));
			float rotDifX = Mathf.Min(rotDifX3, Mathf.Min(rotDifX2, rotDifX1));

			float useRotDif = Mathf.Max(rotDifY, rotDifX);

			bool underThreshold = useRotDif < YRotationThresholdForDelay;

			if (underThreshold)
			{
				if (percentageOfDelayToUse != 0)
				{
					percentageOfDelayToUse = Mathf.Max(0, percentageOfDelayToUse - (Time.deltaTime / TimeToZero));
				}
			}
			else
			{
				if (percentageOfDelayToUse != 1)
				{
					percentageOfDelayToUse = Mathf.Min(1, percentageOfDelayToUse + (Time.deltaTime / TimeToOne));
				}
			}

			if (Time.frameCount != lastFrameCount)
			{
				lastFrameYRot = rotation.eulerAngles.y;
				lastFrameXRot = rotation.eulerAngles.x;
				lastFrameCount = Time.frameCount;
			}

			pastFrameRotations[pastRotationIndex] = rotation;
			pastRotationIndex = (pastRotationIndex + 1) % pastFrameRotations.Length;
		}

		/// <summary>
		/// Gets the rotation prediction.
		/// </summary>
		/// <returns>The rotation prediction.</returns>
		/// <param name = "rotation"></param>
		/// <param name="frameDelay">Frame delay.</param>
		public Quaternion GetRotationPrediction(Quaternion rotation, int frameDelay = 0)
		{
			// We don't have enough to predict on.
			if (pastFrameRotations.Length < 10)
			{
				return Quaternion.identity;
			}

			if (frameDelay < 0)
			{
				frameDelay = 0;
			}

			if (frameDelay > pastFrameRotations.Length)
			{
				frameDelay = pastFrameRotations.Length;
			}

			frameDelay = pastFrameRotations.Length - frameDelay;

			int index = (pastRotationIndex + (pastFrameRotations.Length - 1) + frameDelay) % pastFrameRotations.Length;
			Vector3 currentRotation = rotation.eulerAngles;
			Vector3 difference = (pastFrameRotations[index].eulerAngles - currentRotation);

			difference.x = GetDelta(difference.x);
			difference.y = GetDelta(difference.y);
			difference.z = GetDelta(difference.z);

			difference *= percentageOfDelayToUse;

			currentRotation += difference;

			return Quaternion.Euler(currentRotation);
		}

		/// <summary>
		/// Gets the rotation prediction offset from the current rotation and the predicted rotation.
		/// </summary>
		/// <returns>The rotation prediction offset.</returns>
		/// <param name = "rotation"></param>
		/// <param name="frameDelay">Frame delay.</param>
		public Vector3 GetRotationPredictionOffset(Quaternion rotation, int frameDelay = 0)
		{
			Vector3 headPivotPoint = new Vector3(0, RotationYPoint, RotationZPoint);
			Vector3 headOffsetAfterDelayedRotation = GetRotationPrediction(rotation, frameDelay) * headPivotPoint;
			Vector3 currentHeadOffsetRotation = rotation * headPivotPoint;
			return currentHeadOffsetRotation - headOffsetAfterDelayedRotation;
		}

		#endregion

		#region Private Static Methods

		private static float GetDelta(float dif)
		{
			return (dif > 180) ? dif - 360 : (dif < -180) ? dif + 360 : dif;
		}

		#endregion
	}
}