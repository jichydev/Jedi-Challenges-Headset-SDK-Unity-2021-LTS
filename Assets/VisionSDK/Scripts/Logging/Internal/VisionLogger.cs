namespace Ximmerse.Vision.Internal
{
	/// <summary>
	/// Used to supress logs of the SDK if no logger provided.
	/// </summary>
	public class VisionLogger : IVisionLogger
	{
		#region Public Methods

		/// <summary>
		/// Doesn't log the specified text. Supresses the log.
		/// </summary>
		/// <param name="text">Text.</param>
		public void Log(string text)
		{
		}

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		public void Destroy()
		{
		}

		#endregion
	}
}
