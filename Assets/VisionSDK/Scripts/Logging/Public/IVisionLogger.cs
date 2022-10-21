namespace Ximmerse.Vision
{
	public interface IVisionLogger
	{
		/// <summary>
		/// Doesn't log the specified text. Supresses the log.
		/// </summary>
		/// <param name="text">Text.</param>
		void Log(string text);

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		void Destroy();
	}
}