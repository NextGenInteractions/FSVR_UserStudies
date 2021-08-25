using System;

namespace NextGen.VrManager.Devices
{
    /// <summary>
    /// An enum to describe basic information about a device.
    /// </summary>
    [Flags]
    public enum DeviceCharacteristics : uint
    {
        //
        // Summary:
        //     A default value specifying no flags.
        None = 0,
        //
        // Summary:
        //     The InputDevice is attached to the head.
        HeadMounted = 1,
        //
        // Summary:
        //     The InputDevice has a camera and associated camera tracking information.
        Camera = 2,
        //
        // Summary:
        //     The InputDevice is held in the user's hand. Typically, a tracked controller.
        HeldInHand = 4,
        //
        // Summary:
        //     The InputDevice provides hand tracking information via a Hand input feature.
        HandTracking = 8,
        //
        // Summary:
        //     The InputDevice provides eye tracking information via an Eyes input feature.
        EyeTracking = 16,
        //
        // Summary:
        //     The InputDevice provides 3DOF or 6DOF tracking data.
        TrackedDevice = 32,
        //
        // Summary:
        //     The InputDevice is a game controller.
        Controller = 64,
        //
        // Summary:
        //     The InputDevice is an unmoving reference object used to locate and track other
        //     objects in the world.
        TrackingReference = 128,
        //
        // Summary:
        //     The InputDevice is associated with the left side of the user.
        Left = 256,
        //
        // Summary:
        //     The InputDevice is associated with the right side of the user.
        Right = 512,
        //
        // Summary:
        //     The InputDevice reports software approximated, positional data.
        Simulated6DOF = 1024
    }
}