using UnityEngine;

namespace Ximmerse.Vision
{
	public interface IPositionPrediction
	{
		#region Public Properties

		/// <summary>
		/// Gets the position with prediction.
		/// </summary>
		/// <returns>The prediction.</returns>
		/// <param name="position">Position.</param>
		Vector3 GetPrediction(Vector3 position);

		#endregion
	}
}