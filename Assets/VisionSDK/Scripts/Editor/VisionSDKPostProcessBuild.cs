#if UNITY_IPHONE
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace Ximmerse.Vision.Internal
{
	public static class VisionSDKPostProcessBuild
	{
		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				// Add the supported accessories
				string plistPath = path + "/Info.plist";

				PlistDocument plist = new PlistDocument();
				plist.ReadFromString(File.ReadAllText(plistPath)); 
				PlistElementDict rootDict = plist.root;

				PlistElementArray plistArray = rootDict.CreateArray("UISupportedExternalAccessoryProtocols");
				plistArray.AddString("com.domain.mirage.protocol1");

				File.WriteAllText(plistPath, plist.WriteToString());
			}
		}
	}
}
#endif