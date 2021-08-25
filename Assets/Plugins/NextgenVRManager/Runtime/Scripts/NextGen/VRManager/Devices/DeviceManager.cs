using System;
using System.Collections.Generic;
using System.Linq;
using NextGen.VrManager.Serials;

namespace NextGen.VrManager.Devices
{
    /// <summary>
    /// The DeviceManager is the means in which to get access to devices, or to add new devices
    /// </summary>
    public static class DeviceManager
    {
        private static List<Device> _activeDevices = new List<Device>();
        public static IReadOnlyList<Device> ActiveDevices { get { return _activeDevices; } }

        public static Action<Device> DeviceAdded, DeviceRemoved;

        public static void AddDevice(Device newDevice)
        {
            if(!_activeDevices.Contains(newDevice))
            {
                _activeDevices.Add(newDevice);
                DeviceAdded?.Invoke(newDevice);
            }
        }

        public static void RemoveDevice(string serialNumber)
        {
            for(int i = 0; i < _activeDevices.Count; i++)
            {
                var h = _activeDevices[i];

                if (h.Uid == serialNumber)
                {
                    _activeDevices.RemoveAt(i);
                    DeviceRemoved?.Invoke(h);
                }
            }
        }

        public static void RemoveDevice(Device device)
        {
            _activeDevices.Remove(device);
            DeviceRemoved?.Invoke(device);
        }

        public static Device GetDeviceByUID(string uid)
        {
            return ActiveDevices.FirstOrDefault((device) => device.Uid == uid);
        }
    }
}
