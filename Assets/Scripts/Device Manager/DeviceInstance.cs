//=======================================================================================
// Written by Connor Shipway from NextGen Interactions LLC
//---------------------------------------------------------------------------------------
//
// Name:
//      DeviceInstance
//
// Purpose:
//      The NextGen DeviceInstance is intended to represent a given device recognized by the DeviceManager, and to serve as
//      an interface through to which to modify, utilize, and obtain information about that particular device.
//
// Description:
//      When the DeviceInstance is created, it is manually "booted up" by the DeviceManager.
//      On bootup, the DeviceInstance essentially initializes itself, gathering necessary info from the SteamVR system i.e. serial number and sets its own variables accordingly.
//      On update, the DeviceInstance checks its own variables to determine whether it is currently in a "lost" tracking state, i.e. it hasn't received pose updates for a certain time.
//=======================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace NextGen
{
    public class DeviceInstance : MonoBehaviour
    {
        private DeviceManager manager;

        public uint deviceIndex;
        public string deviceType;
        public DeviceClass deviceClass;
        public string deviceName;
        public float deviceBatteryPercentage;
        public string deviceSerial;
        public string deviceNametag;
        public bool deviceActive;

        public float deviceTimeSinceLastTrack;
        public int deviceFramesSinceLastTrack;
        public bool deviceTrackingLoss;
        public bool deviceTrackingMiniLoss;
        public int deviceTrackingLossesCount = 0;
        public int deviceTrackingMiniLossesCount = 0;

        public Vector3 deviceTrackingPos;
        public Quaternion deviceTrackingRot;

        public bool HasBattery { get { return deviceClass == DeviceClass.Controller || deviceClass == DeviceClass.Puck; } }
        public DeviceCategory GetDeviceCategory { get { return (deviceClass == DeviceClass.Controller || deviceClass == DeviceClass.Puck) ? DeviceCategory.Controller : DeviceCategory.Utility; } }
        public enum DeviceCategory
        {
            Utility,
            Controller
        }
        public enum DeviceClass
        {
            HMD,
            Lighthouse,
            Controller,
            Puck,
            Other
        }

        public DeviceWidget deviceWidget;

        public void Bootup(uint _index, string _name)
        {
            manager = transform.parent.GetComponent<DeviceManager>();

            deviceIndex = _index;
            deviceName = _name;

            var error = ETrackedPropertyError.TrackedProp_Success;

            var type = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty(deviceIndex, ETrackedDeviceProperty.Prop_ControllerType_String, type, 64, ref error);
            deviceType = type.ToString();

            deviceBatteryPercentage = OpenVR.System.GetFloatTrackedDeviceProperty(deviceIndex, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float, ref error);

            var serial = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty(deviceIndex, ETrackedDeviceProperty.Prop_SerialNumber_String, serial, 64, ref error);
            deviceSerial = serial.ToString();
            if (manager.nametags.ContainsKey(deviceSerial))
            {
                deviceNametag = manager.nametags[deviceSerial];
            }
            else
            {
                manager.nametags.Add(deviceSerial, "");
                manager.WriteNametags();
            }

            deviceActive = OpenVR.System.IsTrackedDeviceConnected(deviceIndex);

            if (deviceName.Contains("hmd")) deviceClass = DeviceClass.HMD;
            else if (deviceName.Contains("lh_basestation")) deviceClass = DeviceClass.Lighthouse;
            else if (deviceName.Contains("vr_controller")) deviceClass = DeviceClass.Controller;
            else if (deviceName.Contains("vr_tracker")) deviceClass = DeviceClass.Puck;
            else deviceClass = DeviceClass.Other;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            deviceTimeSinceLastTrack += Time.deltaTime;
            deviceFramesSinceLastTrack++;

            bool lastDeviceTrackingLoss = deviceTrackingLoss;
            bool lastDeviceTrackingMiniLoss = deviceTrackingMiniLoss;

            //deviceTrackingLoss = deviceTimeSinceLastTrack > NextGenDeviceManager.lossTrackingTimeThreshold;
            deviceTrackingLoss = deviceFramesSinceLastTrack > DeviceManager.lossTrackingFramesThreshold;
            //deviceTrackingMiniLoss = deviceTimeSinceLastTrack > NextGenDeviceManager.miniLossTrackingTimeThreshold;
            deviceTrackingMiniLoss = deviceFramesSinceLastTrack > DeviceManager.miniLossTrackingFramesThreshold;

            bool needToRefresh = false;

            if (!lastDeviceTrackingMiniLoss && deviceTrackingMiniLoss)
            {
                deviceTrackingMiniLossesCount++;
                needToRefresh = true;
            }

            if (!lastDeviceTrackingLoss && deviceTrackingLoss)
            {
                deviceTrackingLossesCount++;
                deviceTrackingMiniLossesCount--;
                needToRefresh = true;
            }

            if (needToRefresh)
                if (deviceWidget != null)
                    deviceWidget.Refresh();
        }

        public void SetNametag(string nametag)
        {
            deviceNametag = nametag;
            manager.nametags[deviceSerial] = deviceNametag;
            manager.WriteNametags();
        }
    }
}
