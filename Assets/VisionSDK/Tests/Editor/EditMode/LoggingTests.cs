using UnityEngine;
using NUnit.Framework;
using Ximmerse.Vision;
using Ximmerse.Vision.Internal;

namespace Ximmerse.Vision.Tests
{
	public class LoggingTests
	{
		private bool logCalled = false;

		[Test]
		public void LogDoesntLog()
		{
			// Use the Assert class to test conditions.
			VisionLogger logger = new VisionLogger();

			// Listen to Unitys Log to make sure VisionLogger doesn't log.
			Application.logMessageReceived += LogCallback;

			logger.Log("Test Log");

			Application.logMessageReceived -= LogCallback;

			// This shouldn't be true
			Assert.AreNotEqual(logCalled, true);
		}

		private void LogCallback(string condition, string stackTrace, LogType type)
		{
			logCalled = true;
		}
	}
}