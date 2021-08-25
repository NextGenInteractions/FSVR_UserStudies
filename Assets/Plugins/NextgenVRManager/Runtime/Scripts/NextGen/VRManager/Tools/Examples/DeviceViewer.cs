using System.Collections.Generic;
using NextGen.VrManager.Devices;
using UnityEngine;

namespace NextGen.VrManager.ToolManagement
{
    public class DeviceViewer : Tool
    {
        protected new readonly string _name = "Device Viewer";

        protected new readonly Dictionary<string, DeviceSlot> _deviceSlots =
            new Dictionary<string, DeviceSlot>() {
                {
                    "Tracked Device",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                }
            };

        // Update is called once per frame
        void Update()
        {
            if(Devices["Tracked Device"].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos))
                transform.position = getPos;
            if(Devices["Tracked Device"].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot))
                transform.rotation = getRot;
        }
    }
}