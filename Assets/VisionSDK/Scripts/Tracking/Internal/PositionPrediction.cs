using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.Vision.Internal
{
	public class PositionPrediction : IPositionPrediction
	{
		#region Private Properties

		private const int NumberOfFramePrediction = 4;

		private readonly List<Vector3> historyDifference = new List<Vector3>();
		private readonly List<Vector3> historyPosition = new List<Vector3>();
		private readonly List<Vector3> historyAdjustedPostion = new List<Vector3>();

		private const int ListSize = 3;
		private const float MaxMagnitude = 0.05f;
		private const float JitterThreshold = 0.003f;

		private const float WeightPredictedNow = 0.85f;
		private const float WeightPredictedLast = 0.125f;
		private const float WeightPredictedBeforeLast = 0.025f;

		private Vector3 lastRealPlace;
		private Vector3 lastDifference = Vector3.zero;
		private float lastMagnitude = 0.0f;
		private float lastAcceleration = 0.0f;

		private bool canSkip = true;
		private float frameLookAhead = 0.0f;

		#endregion

		#region Constructor

		public PositionPrediction()
		{
			lastRealPlace = Vector3.zero;

			historyDifference.Add(Vector3.zero);
			historyDifference.Add(Vector3.zero);
			historyDifference.Add(Vector3.zero);

			historyPosition.Add(Vector3.zero);
			historyPosition.Add(Vector3.zero);
			historyPosition.Add(Vector3.zero);

			historyAdjustedPostion.Add(Vector3.zero);
			historyAdjustedPostion.Add(Vector3.zero);
			historyAdjustedPostion.Add(Vector3.zero);
		}

		#endregion

		#region Public Methods

		public Vector3 GetPrediction(Vector3 position)
		{
			// Difference between now and last known posisiton (last frame) 
			Vector3 difference = position - lastRealPlace;

			// Save for later if we don't want to use the predicted position
			Vector3 realDifference = difference;

			// size of the amount it moved from last frame
			float acceleration = (difference - lastDifference).magnitude;

			// difference in the size it moved since last frame
			float accelerationDifference = acceleration - lastAcceleration;

			// Save the "last" based on current
			lastAcceleration = acceleration;
			lastDifference = difference;
			lastRealPlace = position;

			// uh. the size of the movement from last to now
			float magnitude = difference.magnitude;

			// Don't allow too quick of movement? Doing this on magnitude is kinda odd IMO
			if (magnitude > MaxMagnitude)
			{
				magnitude = MaxMagnitude;
			}

			// If after everything is said and done, do we reallllly want to move it?
			bool positionIt = true;

			// We are speeding up or slowing down vs. last frame
			if (accelerationDifference > JitterThreshold || accelerationDifference < -JitterThreshold)
			{
				if (lastMagnitude.Equals(0.0f))
				{
					// Our range is 0 - 4, this makes it based on the size / max to get a percent.
					frameLookAhead = (magnitude / MaxMagnitude) * NumberOfFramePrediction;
				}
				else
				{
					// Going faster then we were
					if (accelerationDifference > 0.0f)
					{
						frameLookAhead += Mathf.Max(frameLookAhead * 2.0f, 0.25f);
					}
					else
					{
						// If we can skip this, we will?
						if (canSkip)
						{
							positionIt = false;
						}

						// Slowing down
						frameLookAhead -= Mathf.Max(frameLookAhead * 0.25f, 0.5f);
					}

					// Then cap it 0 - 4
					frameLookAhead = Mathf.Max(0, Mathf.Min(NumberOfFramePrediction, frameLookAhead));
				}
			}

			// Going about the same speed, and still moving, so cap it
			else if (magnitude > JitterThreshold)
			{
				frameLookAhead = NumberOfFramePrediction;
			}

			// If we are "not moving" set this to 0 or the magnitude
			lastMagnitude = frameLookAhead.Equals(0.0f) ? 0.0f : magnitude;

			// If we didn't really move and we can skip, don't position it.
			if (magnitude < JitterThreshold && canSkip)
			{
				positionIt = false;
			}

			historyDifference.Add(difference.normalized * magnitude);
			historyPosition.Add(position);

			ShiftList(historyDifference);
			ShiftList(historyPosition);

			// Get the total weighted difference and position from the last 3 frames
			Vector3 totalDifference = (historyDifference[2] * WeightPredictedNow) + (historyDifference[1] * WeightPredictedLast) + (historyDifference[0] * WeightPredictedBeforeLast);
			Vector3 totalPosition = (historyPosition[2] * WeightPredictedNow) + (historyPosition[1] * WeightPredictedLast) + (historyPosition[0] * WeightPredictedBeforeLast);

			Vector3 newPosition = Vector3.zero;

			// Triple Weighting ftw? Weight the last 3 frames, then weight based on frame look ahead, and then on all of it again.
			newPosition = totalPosition + (totalDifference * frameLookAhead);

			// Weight the new position again, mostly based on everything above, with a little from the last 2 frames before it.
			newPosition = (newPosition * WeightPredictedNow) + (historyAdjustedPostion[2] * WeightPredictedLast) + (historyAdjustedPostion[1] * WeightPredictedBeforeLast);

			historyAdjustedPostion.Add(newPosition);

			ShiftList(historyAdjustedPostion);

			if (positionIt)
			{
				// We can skip the next one if needed
				canSkip = true;

				// Return the updated position
				return newPosition;
			}
			else
			{
				// 
				historyAdjustedPostion[2] = historyAdjustedPostion[1] + realDifference;

				// Can't skip again
				canSkip = false;

				// Return the "current"
				return historyAdjustedPostion[2];
			}
		}

		#endregion

		#region Private Methods

		private static void ShiftList(IList list)
		{
			while (list.Count > ListSize)
			{
				list.RemoveAt(0);
			}
		}

		#endregion
	}
}