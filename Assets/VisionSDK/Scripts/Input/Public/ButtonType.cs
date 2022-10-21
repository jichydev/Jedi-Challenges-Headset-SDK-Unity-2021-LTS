using Ximmerse.InputSystem;

namespace Ximmerse.Vision
{
    public enum ButtonType
    {
        // HMD Buttons
        HmdMenu = ControllerButton.DpadLeft,
        HmdSelect = ControllerButton.DpadUp,
        HmdBack = ControllerButton.DpadDown,

        // Saber Buttons
        SaberActivate = ControllerButton.PrimaryShoulder,
        SaberControl = ControllerButton.SecondaryShoulder,

        //@EDIT:add Controller Buttons      
        Touchpad = ControllerButton.PrimaryThumb,
        Trigger = ControllerButton.PrimaryTrigger,
        App = ControllerButton.PrimaryShoulder,
        Home = ControllerButton.SecondaryShoulder
    }
}