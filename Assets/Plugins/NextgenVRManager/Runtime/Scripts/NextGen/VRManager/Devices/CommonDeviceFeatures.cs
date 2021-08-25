using UnityEngine;

namespace NextGen.VrManager.Devices {
    /// <summary>
    /// A reference to common device features, that many different types of devices may share.
    /// </summary>
    public static class CommonDeviceFeatures
    {
        public static DeviceFeatureUsage<float> batteryLevel = new DeviceFeatureUsage<float> { name = "BatteryLevel" };

        //*************************
        //Tracking
        //*************************
        public static DeviceFeatureUsage<bool> isTracked = new DeviceFeatureUsage<bool> { name = "IsTracked" };
        public static DeviceFeatureUsage<Vector3> devicePosition = new DeviceFeatureUsage<Vector3> { name = "DevicePosition" };
        public static DeviceFeatureUsage<Quaternion> deviceRotation = new DeviceFeatureUsage<Quaternion> { name = "DeviceRotation" };
        public static DeviceFeatureUsage<Hands.Hand> handData = new DeviceFeatureUsage<Hands.Hand> { name = "HandData" };

        //*************************
        //Input
        //*************************

        public static DeviceFeatureUsage<bool> triggerButton = new DeviceFeatureUsage<bool> { name = "TriggerButton" };
        public static DeviceFeatureUsage<float> trigger = new DeviceFeatureUsage<float> { name = "Trigger" };

        public static DeviceFeatureUsage<bool> gripButton = new DeviceFeatureUsage<bool> { name = "GripButton" };

        public static DeviceFeatureUsage<bool> primary2DAxisTouch = new DeviceFeatureUsage<bool> { name = "Primary2DAxisTouch" };
        public static DeviceFeatureUsage<bool> primary2DAxisClick = new DeviceFeatureUsage<bool> { name = "Primary2DAxisClick" };
        public static DeviceFeatureUsage<Vector2> primary2DAxis = new DeviceFeatureUsage<Vector2> { name = "Primary2DAxis" };

        public static DeviceFeatureUsage<bool> menuButton = new DeviceFeatureUsage<bool> { name = "MenuButton" };
    }
}