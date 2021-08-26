using NextGen.VrManager.Devices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextGen.VrManager.DebugTools
{
    public class JitterLossCounter : MonoBehaviour
    {
        public static JitterLossCounter Instance;

        public const float jitterThreshold = 0.1f;
        public const float lossThreshold = 1f;

        [SerializeField]
        Dictionary<Device, JitterLossCount> jitterLossCounts = new Dictionary<Device, JitterLossCount>();

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            foreach (Device device in DeviceManager.ActiveDevices)
            {
                OnDeviceAdded(device);
            }

            DeviceManager.DeviceAdded += OnDeviceAdded;
            DeviceManager.DeviceRemoved += OnDeviceRemoved;
        }

        private void OnDisable()
        {
            foreach (var j in jitterLossCounts)
            {
                Destroy(j.Value);
            }

            jitterLossCounts.Clear();

            DeviceManager.DeviceAdded -= OnDeviceAdded;
            DeviceManager.DeviceRemoved -= OnDeviceRemoved;
        }

        private void OnDeviceAdded(Device device)
        {
            if (device.GetFeatures().Contains((DeviceFeatureUsage)CommonDeviceFeatures.isTracked))
            {
                JitterLossCount jitterLossCount = new GameObject(device.Name).AddComponent<JitterLossCount>();
                jitterLossCount.Init(device);
                jitterLossCount.transform.SetParent(transform);
                jitterLossCounts[device] = jitterLossCount;
            }
        }

        private void OnDeviceRemoved(Device device)
        {
            if (jitterLossCounts.ContainsKey(device))
            {
                Destroy(jitterLossCounts[device].gameObject);

                jitterLossCounts.Remove(device);
            }
        }

        public static JitterLossCount GetJitterLossCount(Device d)
        {
            JitterLossCount toRet = null;

            if (Instance.jitterLossCounts.ContainsKey(d))
                toRet = Instance.jitterLossCounts[d];

            return toRet;
        }
    }
}
