using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;
using NextGen.VrManager.Devices.SteamVr;

namespace NextGen.VrManager.Devices.UnityXr
{
    /// <summary>
    /// Responsible for populating and updating all UnityXRDevices
    /// </summary>
    public class UnityXrDeviceManager : MonoBehaviour
    {
        public float updateInterval = 1;
        private Coroutine pollDevicesAtInterval;

        private void Start()
        {
            PollDevices();
            pollDevicesAtInterval = StartCoroutine(PollDevicesAtInterval());
        }

        private void OnDisable()
        {
            StopCoroutine(pollDevicesAtInterval);
        }

        // Update is called once per frame
        void PollDevices()
        {
            List<InputDevice> polledDevices = new List<InputDevice>();
            InputDevices.GetDevices(polledDevices);

            List<string> polledSerialNumbers = new List<string>();

            foreach (InputDevice polledDevice in polledDevices)
            {
                polledSerialNumbers.Add(polledDevice.serialNumber);

                UnityXrDevice device = (UnityXrDevice)DeviceManager.ActiveDevices.FirstOrDefault((device) => { return string.IsNullOrEmpty(polledDevice.serialNumber) ? device.Name == polledDevice.name : device.Uid == polledDevice.serialNumber; });

                if (device == null)
                {
                    device = new UnityXrDevice();

                    device.SetData(new InputDeviceWrapper(polledDevice));

                    DeviceManager.AddDevice(device);
                }
            }

            for(int i = 0; i < DeviceManager.ActiveDevices.Count(); i++)
            {
                Device device = DeviceManager.ActiveDevices[i];

                if(device.GetType() == typeof(UnityXrDevice))
                {
                    if (!polledSerialNumbers.Contains(device.Uid))
                    {
                        DeviceManager.RemoveDevice(device.Uid);
                    }
                    else
                    {
                        ((UnityXrDevice)device).RefreshFeatures();
                    }
                }
            }
        }

        IEnumerator PollDevicesAtInterval()
        {
            var wait = new WaitForSeconds(updateInterval);

            while (true)
            {
                yield return wait;
                PollDevices();
            }
        }
    }
}