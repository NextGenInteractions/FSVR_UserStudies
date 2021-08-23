using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;

namespace NextGen.Tools
{
    public class Mannequin : Tool
    {
        public Transform ikTargetsParent;
        public Transform ikTargetsFallbacksParent;

        public List<Transform> ikTargets = new List<Transform>();
        public List<Transform> ikTargetsFallbacks = new List<Transform>();

        private void Awake()
        {
            _name = "Mannequin";

            _deviceSlots =
            new Dictionary<string, DeviceSlot>() {
                {
                    "Head",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                },
                {
                    "Pelvis",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                },
                {
                    "Left Arm",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                },
                {
                    "Right Arm",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                },
                {
                    "Left Leg",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                },
                {
                    "Right Leg",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                }
            };

            for (int i = 0; i < ikTargetsParent.childCount; i++)
            {
                ikTargets.Add(ikTargetsParent.GetChild(i));
            }

            for (int i = 0; i < ikTargetsFallbacksParent.childCount; i++)
            {
                ikTargetsFallbacks.Add(ikTargetsFallbacksParent.GetChild(i));
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Get tracking from tracked devices.
            //GetPosAndRot();
            UpdateIkTargets();
        }

        public void GetPosAndRot()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Head"))
                {
                    if (Devices["Head"].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos))
                    {
                        if (getPos != Vector3.zero) //If getPos is 0, then tracking is lost and we want to disregard the tracked position until it's back.
                        {
                            transform.position = getPos;
                            if (Devices["Head"].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot))
                                transform.rotation = getRot;
                        }
                    }
                }
            }
        }

        public void UpdateIkTargets()
        {
            for(int i = 0; i < 6; i++)
            {
                string s = IndexToString(i);

                Vector3 position = ikTargetsFallbacks[i].position;
                Quaternion rotation = ikTargetsFallbacks[i].rotation;

                if (Devices != null && Devices.ContainsKey(s))
                {
                    Devices[s].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out position);
                    Devices[s].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out rotation);
                }

                ikTargets[i].SetPositionAndRotation(position, rotation);
            }
        }

        public string IndexToString(int index)
        {
            string toRet = "";

            switch(index)
            {
                case 0: toRet = "Head"; break;
                case 1: toRet = "Pelvis"; break;
                case 2: toRet = "Left Arm"; break;
                case 3: toRet = "Right Arm"; break;
                case 4: toRet = "Left Leg"; break;
                case 5: toRet = "Right Leg"; break;
            }

            return toRet;
        }
}
}
