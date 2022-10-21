using UnityEngine;
using System.Runtime.InteropServices;


namespace Ximmerse.InputSystem
{

    public partial class XDevicePlugin
    {

        internal class NativeMethods_Mirage
        {

            private const string pluginName = LIB_XDEVICE;

#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS)
            [DllImport(pluginName,CallingConvention=CallingConvention.Cdecl)]
            public static extern void SetMaxBleControllers(int num);

            [DllImport(pluginName,CallingConvention=CallingConvention.Cdecl)]
            public static extern int GetMaxBleControllers();

            [DllImport(pluginName, CallingConvention=CallingConvention.Cdecl)]
            public static extern void startMatch();
            [DllImport(pluginName, CallingConvention=CallingConvention.Cdecl)]
            public static extern void stopMatch();

#elif UNITY_EDITOR_WIN
            public static void SetMaxBleControllers(int num)
            {
            }

            public static int GetMaxBleControllers()
            {
                return 2;
            }
#elif !UNITY_EDITOR && UNITY_ANDROID
            public static void SetMaxBleControllers(int num)
            {
                if (XDevicePlugin.s_XDeviceApi != null)
                {
                    XDevicePlugin.s_XDeviceApi.CallStatic("setControllerCount", num);
                }
            }

            public static int GetMaxBleControllers()
            {
                if (XDevicePlugin.s_XDeviceApi != null) 
                {
                    return XDevicePlugin.s_XDeviceApi.CallStatic<int>("getControllerCount");
                }
                return 2;
            }

            public static void BLEStartScan()
            {
                if (XDevicePlugin.s_XDeviceApi != null) 
                {
                    XDevicePlugin.s_XDeviceApi.CallStatic("setControllerPairingMode", true);
                }
            }

            public static void BLEStopScan()
            {
                if (XDevicePlugin.s_XDeviceApi != null) 
                {
                    XDevicePlugin.s_XDeviceApi.CallStatic("setControllerPairingMode", false);
                }
            }
#endif
        }

        public static void SetMaxBleConnection(int num)
        {
            NativeMethods_Mirage.SetMaxBleControllers(num);
        }

        public static int GetMaxBleConnection()
        {
            return NativeMethods_Mirage.GetMaxBleControllers();
        }

        public static void StartBLEScan()
        {
#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS)
            NativeMethods_Mirage.startMatch();
#elif UNITY_EDITOR_WIN
            // no methods implemented in dll
#elif !UNITY_EDITOR && UNITY_ANDROID
            NativeMethods_Mirage.BLEStartScan();
#endif
        }

        public static void StopBLEScan()
        {
#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS)
            NativeMethods_Mirage.stopMatch();
#elif UNITY_EDITOR_WIN
            // no methods implemented in dll
#elif !UNITY_EDITOR && UNITY_ANDROID
            NativeMethods_Mirage.BLEStopScan();
#endif
        }
    }
}
