using NUnit.Framework;
using Ximmerse.Vision;
using Ximmerse.Vision.Internal;

namespace Ximmerse.Vision.Tests
{
	public class SettingsManagerTests
	{
		[Test]
		public void DefaultAndUnsupportedDevice()
		{
			// Use the Assert class to test conditions.
			SettingsManager manager = new SettingsManager(new VisionLogger());

			// Make sure the Device isn't null or default (it should use the first found if none match)
			Assert.AreNotEqual(manager.DeviceSettings, null);
			Assert.AreNotEqual(manager.DeviceSettings, new DeviceSettings());

			// Make sure the Headset isn't null or default (it should use the first found if none match)
			Assert.AreNotEqual(manager.HeadsetSettings, null);
			Assert.AreNotEqual(manager.HeadsetSettings, new HeadsetSettings());
		}
	}
}