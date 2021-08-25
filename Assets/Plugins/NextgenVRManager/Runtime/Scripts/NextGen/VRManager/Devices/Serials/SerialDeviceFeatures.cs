using UnityEngine;

namespace NextGen.VrManager.Devices.Serials
{
    public static class SerialDeviceFeatures
    {
        public static DeviceFeatureUsage<Vector2> touchpad = new DeviceFeatureUsage<Vector2> { name = "Touchpad" };
        public static DeviceFeatureUsage<bool> touchpadTouch = new DeviceFeatureUsage<bool> { name = "TouchpadTouch" };
    }
}