using NextGen.VrManager.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NextGen.VrManager.DebugTools
{
    public class TrackedDeviceVisualizer : MonoBehaviour
    {
        [SerializeField]
        public Dictionary<Device, SimpleDeviceVisualization> deviceVisualizations = new Dictionary<Device, SimpleDeviceVisualization>();


        [SerializeField]
        List<DeviceTypePrefab> deviceTypePrefabs = new List<DeviceTypePrefab>();

        private void OnEnable()
        {

            foreach (Device device in DeviceManager.ActiveDevices)
            {
                OnDeviceAdded(device);
            }

            DeviceManager.DeviceAdded += OnDeviceAdded;
            DeviceManager.DeviceRemoved += OnDeviceRemoved;
            DeviceMetadataManager.DeviceMetadataChanged += OnDeviceMetadataChanged;
        }

        private void OnDisable()
        {
            foreach (var t in deviceVisualizations)
            {
                Destroy(t.Value.gameObject);
            }

            deviceVisualizations.Clear();

            DeviceManager.DeviceAdded -= OnDeviceAdded;
            DeviceManager.DeviceRemoved -= OnDeviceRemoved;
            DeviceMetadataManager.DeviceMetadataChanged -= OnDeviceMetadataChanged;
        }

        private void OnDeviceAdded(Device device)
        {
            SimpleDeviceVisualization trackedObject = null;
            var matchingDeviceTypePrefab = deviceTypePrefabs.FirstOrDefault((deviceTypePrefab) => { return deviceTypePrefab.type == device.Metadata?.Type; });

            if(matchingDeviceTypePrefab.prefab != null)
                trackedObject = Instantiate(matchingDeviceTypePrefab.prefab, transform);

            if (trackedObject)
            {
                deviceVisualizations[device] = trackedObject;
                SetLayerRecursively(trackedObject.gameObject, 7);
                trackedObject.Init(device);
            }
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            if (obj.layer == 0)
                obj.layer = 7;

            for(int i = 0; i < obj.transform.childCount; i++)
            {
                SetLayerRecursively(obj.transform.GetChild(i).gameObject, layer);
            }
        }

        private void OnDeviceMetadataChanged(string serialNumber, DeviceMetadata metadata)
        {
            var device = DeviceManager.GetDeviceByUID(serialNumber);

            if(device != null)
            {
                OnDeviceRemoved(device);
                OnDeviceAdded(device);
            }
        }

        private void OnDeviceRemoved(Device device)
        {
            if(deviceVisualizations.ContainsKey(device))
            {
                Destroy(deviceVisualizations[device].gameObject);

                deviceVisualizations.Remove(device);
            }
        }

        [Serializable]
        private struct DeviceTypePrefab
        {
            public Devices.DeviceType type;
            public SimpleDeviceVisualization prefab;
        }
    }
}
