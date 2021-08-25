using Leap;
using UnityEngine;

namespace NextGen.VrManager.Devices.LeapMotion {
    public static class LeapDeviceFeatures
    {
        public static DeviceFeatureUsage<Hand> handData = new DeviceFeatureUsage<Hand> { name = "HandData" };
    }
}