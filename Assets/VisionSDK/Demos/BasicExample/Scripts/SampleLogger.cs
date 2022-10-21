using UnityEngine;
using UnityEngine.UI;
using Ximmerse.Vision;

namespace Ximmerse.Example
{
	public class SampleLogger : MonoBehaviour, IVisionLogger
	{
		public Text TextField;

		#region Unity Methods

		private void Start()
		{
			Application.logMessageReceived += HandleUnityLog;
		}

		private void OnDestroy()
		{
			Application.logMessageReceived -= HandleUnityLog;
		}

		#endregion

		#region IVisionLogger implementation

		public void Log(string text)
		{
			if (TextField == null)
			{
				return;
			}

			TextField.text = text + "\n" + TextField.text;

			if (TextField.text.Length > 2048)
			{
				TextField.text = TextField.text.Substring(0, 2048);
			}

            Debug.Log("VisionSDKLog:" + text);
		}

		public void Destroy()
		{
		}

		#endregion

		#region Private Methods

		private void HandleUnityLog(string logString, string stackTrace, LogType type)
		{
			if (type == LogType.Exception)
			{
				Log("Exception: " + logString + "\n" + stackTrace.Substring(0, 128)); // Stacks are mad long
			}
			else if (type == LogType.Error)
			{
				Log("Error: " + logString);
			}
		}

		#endregion

	}
}