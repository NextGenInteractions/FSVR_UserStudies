using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace NextGen
{
    public class TrackedObject : MonoBehaviour
    {
        [SerializeField] private DeviceInstance device;
        public DeviceInstance Device
        {
            set
            {
                device = value;
            }
        }
        public DeviceInstance setDevice;
        public string setDeviceByNametag;
        [HideInInspector] public DeviceManager manager;


        [Tooltip("If not set, relative to parent")]
        public Transform origin;

        // Start is called before the first frame update
        void Start()
        {
            manager = DeviceManager.singleton;
        }

        // Update is called once per frame
        void Update()
        {
            if (setDevice != null)
            {
                Device = setDevice;
                setDevice = null;
            }

            if (setDeviceByNametag != "")
            {
                if (manager.nametags.ContainsValue(setDeviceByNametag))
                {
                    Device = manager.GetDeviceWithNametag(setDeviceByNametag);
                    setDeviceByNametag = "";
                }
            }

            if (device != null)
            {
                if (origin == null)
                {
                    transform.position = device.deviceTrackingPos;
                    transform.rotation = device.deviceTrackingRot;
                }
                else
                {
                    transform.position = origin.transform.TransformPoint(device.deviceTrackingPos);
                    transform.rotation = origin.rotation * device.deviceTrackingRot;
                }
            }
        }
    }
}
