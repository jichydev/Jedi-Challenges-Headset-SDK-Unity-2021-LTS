//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

namespace Ximmerse.InputSystem {

	public partial class XDevicePlugin {

        public const int
            XIM_DK4       = 0x4000,
            XIM_DK4_DIS01 = 0x4001,
            XIM_RD06 =      0x4002, 
            XIM_CV1 = 0x4010,
            XIM_CV1_HTC   = 0x4011,
            XIM_CV1_SAM   = 0x4012;


        public const int
			ID_CONTEXT=0xFF
		;

		public const int
        kField_None = 0
            // Bool
            ,kField_CtxBoolOffset = 11000
            ,kField_CtxCanProcessInputEventBool = 11000
            ,kField_CtxBoolMax = 11001
            ,kField_BoolOffset = 1000
            ,kField_AutoProcessInputEventBool = 1000
            ,kField_CanProcessInputEventBool = 1001
            ,kField_CanProcessAxisEventBool = 1002
            ,kField_CanProcessKeyEventBool = 1003
            ,kField_CanProcessPositionEventBool = 1004
            ,kField_CanProcessAccelerometerEventBool = 1005
            ,kField_CanProcessRotationEventBool = 1006
            ,kField_CanProcessGyroscopeEventBool = 1007
            ,kField_CanCheckAxesRangeBool = 1008
            ,kField_CanMapAxesToButtonsBool = 1009
            ,kField_IsAbsRotationBool = 1010
            ,kField_IsDeviceSleepBool = 1011
            ,kField_IMUCalibrationUpdateBool = 1012
            ,kField_BoolMax = 1013
            // Int
            , kField_CtxIntOffset = 12000
            ,kField_CtxSdkVersionInt = 12000
            ,kField_CtxDeviceVersionInt = 12001
            ,kField_CtxPlatformVersionInt = 12002
            ,kField_CtxLogLevelInt = 12003
            ,kField_CtxVIDInt = 12004
            ,kField_CtxPIDInt = 12005
            ,kField_CtxCustomerIDInt = 12006
            ,kField_CtxSmoothLevelInt = 12007
            ,kField_CtxIntMax = 12008
            ,kField_IntOffset = 2000
            ,kField_NumAxesInt = 2001
            ,kField_NumButtonsInt = 2002
            ,kField_ErrorCodeInt = 2003
            ,kField_ConnectionStateInt = 2004
            ,kField_BatteryLevelInt = 2005
            ,kField_TrackingResultInt = 2006
            ,kField_TrackingOriginInt = 2007
            ,kField_FwVersionInt = 2008
            ,kField_BlobIDInt = 2009
            ,kField_FPSInt = 2010
            ,kField_IntMax = 2011
            // Float
            ,kField_FloatOffset = 3000
            ,kField_PositionScaleFloat = 3000
            ,kField_TrackerHeightFloat = 3001
            ,kField_TrackerDepthFloat = 3002
            ,kField_TrackerPitchFloat = 3003
            ,kField_TriggerAsButtonThresholdFloat = 3004
            ,kField_AxisAsButtonThresholdFloat = 3005
            ,kField_AxisDeadzoneThresholdFloat = 3006
            ,kField_AccelScaleFloat = 3007
            ,kField_GyroScaleFloat = 3008
            ,kField_BatteryTemperatureFloat = 3009
            ,kField_FloatMax = 3010
            // Object
            ,kField_ObjectOffset = 4000
            ,kField_TRSObject = 4000
            ,kField_DeviceInfoObject = 4001
            ,kField_AddressObject = 4002
            ,kField_DisplayNameObject = 4003
            ,kField_ModelNameObject = 4004
            ,kField_SerialNumberObject = 4005
            ,kField_FirmwareRevisionObject = 4006
            ,kField_HardwareRevisionObject = 4007
            ,kField_SoftwareRevisionObject = 4008
            ,kField_ManufacturerNameObject = 4009
            ,kField_HardwareSNObject = 4010
            ,kField_FPGAVersionObject = 4011
            ,kField_HMDSN8Object = 4012
            ,kField_ObjectMax = 4013,
			// Message
			kMessage_TriggerVibration          = 1,
			kMessage_RecenterSensor            = 2,
			kMessage_SleepMode                 = 3,
			kMessage_UpdateDisplayStrings      = 4,
			kMessage_DeviceOperation           = 5,
            kMessage_ChangeBlobColorTemp       = 6,
            kMessage_ChangeBlobColorDefault    = 7,
            kMessage_SetDeviceName             = 8,
            kMessage_SetModelName              = 9,
            kMessage_SetFPGA                   = 10,
            kMessage_UpdateCalibrationParam    = 11,
            kMessage_SetChannel                = 12,
            kMessage_SetCalibration            = 13,
            //
            OK                                 = 0
            ;

        public const int
            FEATURE_BLOB_AUTO = 0x0001,
            FEATURE_POSITION_PREDICTION = 0x0002
            ;

        public const int
           CALIBRATION_OPEN = 1,
           CALIBRATION_CLOSE = 2,
           CALIBRATION_WRITE_SUCCESS = 3,
           CALIBRATION_OPEN_FAIL = -1,
           CALIBRATION_CLOSE_FAIL = -2,
           CALIBRATION_WRITE_FAIL = -3;
    }

    public enum LOGLevel
    {
        LOG_VERBOSE = 0,
        LOG_DEBUG = 1,
        LOG_INFO = 2,
        LOG_WARN = 3,
        LOG_ERROR = 4,
        LOG_OFF = 5,
    };

    // Reference from GoogleVR.

    /// <summary>
    /// Represents the device's current connection state.
    /// </summary>
    public enum DeviceConnectionState {
		/// <summary>
		/// Indicates that the device is disconnected.
		/// </summary>
		Disconnected,
		/// <summary>
		/// Indicates that the host is scanning for devices.
		/// </summary>
		Scanning,
		/// <summary>
		/// Indicates that the device is connecting.
		/// </summary>
		Connecting,
		/// <summary>
		/// Indicates that the device is connected.
		/// </summary>
		Connected,
		/// <summary>
		/// Indicates that an error has occurred.
		/// </summary>
		Error,
	};

	[System.Flags]
	public enum TrackingResult{
		NotTracked      =    0,
		RotationTracked = 1<<0,
		PositionTracked = 1<<1,
		PoseTracked     = (RotationTracked|PositionTracked),
		RotationEmulated = 1<<2,
		PositionEmulated = 1<<3,
	};

	public enum XimmerseButton {
		// Standard
		Touch   = ControllerRawButton.LeftThumbMove,
		Click   = DpadClick|DpadUp|DpadDown|DpadLeft|DpadRight,
		App     = ControllerRawButton.Back,
		Home    = ControllerRawButton.Start,
		// Touchpad To Dpad
		DpadUp    = ControllerRawButton.DpadUp,
		DpadDown  = ControllerRawButton.DpadDown,
		DpadLeft  = ControllerRawButton.DpadLeft,
		DpadRight = ControllerRawButton.DpadRight,
		DpadClick = ControllerRawButton.LeftThumb,
		// Gestures
		SwipeUp     = ControllerRawButton.LeftThumbUp,
		SwipeDown   = ControllerRawButton.LeftThumbDown,
		SwipeLeft   = ControllerRawButton.LeftThumbLeft,
		SwipeRight  = ControllerRawButton.LeftThumbRight,
		SlashUp    = ControllerRawButton.RightThumbUp,
		SlashDown  = ControllerRawButton.RightThumbDown,
		SlashLeft  = ControllerRawButton.RightThumbLeft,
		SlashRight = ControllerRawButton.RightThumbRight,
		// Ximmerse Ex
		Trigger = ControllerRawButton.LeftTrigger,
		GripL   = ControllerRawButton.LeftShoulder,
		GripR   = ControllerRawButton.RightShoulder,
		Grip    = GripL|GripR,
	}

	public static class XimmerseExtension {

		public static bool GetButton(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButton((uint)buttonMask);
		}

		public static bool GetButtonDown(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButtonDown((uint)buttonMask);
		}

		public static bool GetButtonUp(this ControllerInput thiz,XimmerseButton  buttonMask) {
			if(thiz==null) {
				return false;
			}
			return thiz.GetButtonUp((uint)buttonMask);
		}
	}
	
    public enum DeviceOperation {
		Close,
		Open,
	}
}
