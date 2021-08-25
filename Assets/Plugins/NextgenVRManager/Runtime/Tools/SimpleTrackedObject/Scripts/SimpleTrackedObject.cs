using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;

namespace NextGen.Tools
{
    public class SimpleTrackedObject : Tool
    {
        private void Awake()
        {
            _name = name;

            _deviceSlots =
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
        }

        // Update is called once per frame
        void Update()
        {
            //Get tracking from paired tracked device.
            GetPosAndRot();
        }

        public void GetPosAndRot()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Tracked Device"))
                {
                    if (Devices["Tracked Device"].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos))
                    {
                        if (getPos != Vector3.zero) //If getPos is 0, then tracking is lost and we want to disregard the tracked position until it's back.
                        {
                            SetPosition(getPos);
                            if (Devices["Tracked Device"].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot))
                                SetRotation(getRot);
                        }
                    }
                }
            }
        }
    }
}
