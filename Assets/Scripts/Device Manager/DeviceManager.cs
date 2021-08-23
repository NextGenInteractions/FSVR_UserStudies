//=======================================================================================
// Written by Connor Shipway from NextGen Interactions LLC
//---------------------------------------------------------------------------------------
//
// Name:
//      DeviceManager
//
// Purpose:
//      The NextGen DeviceManager is intended to provide an inclusive tool to handle the gathering, display, management, and organization
//      of SteamVR devices in the context of NextGen projects. Such devices include lighthouses, HMDs, controllers, pucks, and custom
//      tracked objects built in-house by NextGen.
//
// Description:
//      Gathers SteamVR devices using the SteamVR library, and instantiates a GameObject under this one for each device gathered in this way.
//      Each child GameObject is named based on the name of the device and contains a DeviceInstance component containing additional info about the device.
//      The DeviceManager also handles listening for new poses for such devices, and passing along such pose data to their corresponding DeviceInstances.
//      Devices can work with a nametag system wherein a custom "nametag" string can be associated with a given device by its serial number.
//      The DeviceManager also handles reading such nametag config files from the appropriate filepath, as well as writing new/revised nametags.
//=======================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System.IO;
using UnityEditor;

namespace NextGen
{
    public class DeviceManager : MonoBehaviour
    {
        /// <summary>
        /// Static reference to the instantiated DeviceManager, used in other static methods for this class.
        /// </summary>
        public static DeviceManager singleton;

        /// <summary>
        /// List of all DeviceInstance components of the children gameObjects under this one, each of which represents a SteamVR device.
        /// </summary>
        List<DeviceInstance> devices = new List<DeviceInstance>();

        /// <summary>
        /// Dictionary of nametags where the key for each nametag is the corresponding device's serial number.
        /// </summary>
        public Dictionary<string, string> nametags = new Dictionary<string, string>();


        [Header("Scene References")]

        [Tooltip("The DeviceManagerUI component used to visualize devices for this DeviceManager, if any.")]
        public DeviceManagerUI ui;

        [Tooltip("The prefab to instantiate for each device, if visualizing this DeviceManager with a UI.")]
        public GameObject deviceWidgetPrefab;

        private string nametagsFilepath;

        [Header("Settings")]

        //Using time for this creates some glitches, even though it would be preferable. Using frames for now.
        //public static float lossTrackingTimeThreshold = .5f;
        //public static float miniLossTrackingTimeThreshold = .16667f;
        public static int lossTrackingFramesThreshold = 30;
        public static int miniLossTrackingFramesThreshold = 10;

        //This binds the SteamVR "New Poses" event to the OnNewPoses method of this class.
        readonly SteamVR_Events.Action newPosesAction;
        protected DeviceManager()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        /// <summary>
        /// Called whenever SteamVR triggers its New Poses event, this method uses the pose data passed along by SteamVR
        /// as a parameter and propagates that data along to the children DeviceInstances under this GameObject.
        /// </summary>
        /// <param name="poses">Pose data as handled by SteamVR.</param>
        protected virtual void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            int i = 0;
            foreach (DeviceInstance device in devices)
            {
                if (poses[i].bDeviceIsConnected && poses[i].bPoseIsValid)
                {
                    var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);

                    device.deviceTimeSinceLastTrack = 0;
                    device.deviceFramesSinceLastTrack = 0;

                    device.deviceTrackingPos = pose.pos;
                    device.deviceTrackingRot = pose.rot;
                }

                i++;
            }
        }

        private void Awake()
        {
            singleton = this;
            OnEnable();
        }

        void OnEnable()
        {
            var render = SteamVR_Render.instance;
            if (render == null)
            {
                enabled = false;
                return;
            }

            newPosesAction.enabled = true;
        }

        void OnDisable()
        {
            newPosesAction.enabled = false;
        }

        void Start()
        {
            //Sets nametagsFilepath based on platform.
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    nametagsFilepath = TextManip.FilePathUp(Application.dataPath) + "/Builds/lighthouseManagerNametags.txt";
                    break;

                case RuntimePlatform.WindowsPlayer:
                    nametagsFilepath = TextManip.FilePathUp(Application.dataPath) + "/lighthouseManagerNametags.txt";
                    break;
            }

            ReadNametags();
            ReadDevices();
        }

        /// <summary>
        /// <para>Reads SteamVR devices using the SteamVR library, and instantiates new children GameObjects under this one to represent each device read in this way.</para>
        /// <para>Each child GameObject contains a DeviceInstance component which will contain information about each device and handle the reception of new info pertaining to that device.</para>
        /// <para>If a DeviceManagerUI is being used, it will also instantiate a DeviceWidget in that UI.</para>
        /// </summary>
        /// <seealso cref="DeviceInstance"/>
        void ReadDevices()
        {
            Debug.Log("DeviceManager: Reading SteamVR devices...");

            var error = ETrackedPropertyError.TrackedProp_Success;
            ETrackedDeviceProperty deviceName = ETrackedDeviceProperty.Prop_RenderModelName_String;

            for (uint i = 0; i < 16; i++)
            {
                var name = new System.Text.StringBuilder((int)64);
                OpenVR.System.GetStringTrackedDeviceProperty(i, deviceName, name, 64, ref error);

                string toString = name.ToString();
                if (toString != "")
                {
                    DeviceInstance device = new GameObject(toString).AddComponent<DeviceInstance>();
                    device.transform.parent = transform;
                    device.Bootup(i, toString);
                    devices.Add(device);

                    if (ui != null)
                    {
                        DeviceWidget deviceWidget = Instantiate(deviceWidgetPrefab, ui.deviceWidgets).GetComponent<DeviceWidget>();
                        deviceWidget.Bootup(device);
                        device.deviceWidget = deviceWidget;
                    }
                }
            }

            if (ui != null) ui.Bootup();
        }

        /// <summary>
        /// Populates the "nametags" dictionary using data contained in the nametags config file.
        /// </summary>
        public void ReadNametags()
        {
            string[] lines = File.ReadAllLines(nametagsFilepath);
            foreach (string line in lines)
            {
                try
                {
                    int index = line.IndexOf(':');
                    string serial = line.Substring(0, index);
                    string nametag = line.Substring(index + 1);
                    nametags.Add(serial, nametag);
                }
                catch { }
            }
        }

        /// <summary>
        /// Writes or rewrites the nametags config file from scratch, using the current data contained in the "nametags" dictionary.
        /// </summary>
        public void WriteNametags()
        {
            StreamWriter writer = new StreamWriter(nametagsFilepath);
            foreach (KeyValuePair<string, string> pair in nametags)
            {
                writer.WriteLine(string.Format("{0}:{1}", pair.Key, pair.Value));
            }
            writer.Close();
        }

        /// <summary>
        /// Returns a DeviceInstance with a nametag the same as the argument given, if any such devices exist.
        /// </summary>
        /// <param name="nametag">The nametag to check devices for.</param>
        public DeviceInstance GetDeviceWithNametag(string nametag)
        {
            foreach (DeviceInstance device in devices)
            {
                if (device.deviceNametag == nametag) return device;
            }
            return null;
        }
    }
}


