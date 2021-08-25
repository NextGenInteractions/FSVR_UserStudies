using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NextGen.VrManager.Devices
{
    public class SteamVrDeviceManager : MonoBehaviour
    {
        public static IDictionary<string, SteamVrDevice> Devices { get; private set; } = new Dictionary<string, SteamVrDevice>();

        public float updateInterval = 1;
        Coroutine getDevicesAtIntervalInstance;

        private void Start()
        {
            GetDevices();
            getDevicesAtIntervalInstance = StartCoroutine(GetDevicesAtInterval());
        }

        private void OnDisable()
        {
            StopCoroutine(getDevicesAtIntervalInstance);
        }

        // Update is called once per frame
        void GetDevices()
        {
            for (uint i = 0; i < 16; i++)
            {
                var device = new SteamVrDevice(i);

                if(device.isValid)
                {
                    Devices[device.serialNumber] = device;
                }
            }
        }

        IEnumerator GetDevicesAtInterval()
        {
            var wait = new WaitForSeconds(updateInterval);

            while (true)
            {
                yield return wait;
                GetDevices();
            }
        }
    }
}