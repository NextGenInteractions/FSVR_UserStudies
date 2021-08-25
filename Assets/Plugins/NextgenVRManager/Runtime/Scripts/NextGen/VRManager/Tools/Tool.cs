using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static NextGen.VrManager.ToolManagement.ToolManager;

namespace NextGen.VrManager.ToolManagement
{
    /// <summary>
    /// Tools are the MonoBehaviour implementation of ITool, check ITool for a better understanding of what Tools are
    /// </summary>
    public abstract class Tool : MonoBehaviour, ITool
    {
        protected Dictionary<string, DeviceSlot> _deviceSlots = new Dictionary<string, DeviceSlot>();
        /// <summary>
        /// A dictionary of this tool's device slots, where the key for each item is the name of the device slot.
        /// For example, a paintbrush might have only one slot for a tracked device with button input,
        /// whereas a mannequin might have six slots for six tracked devices.
        /// </summary>
        public IReadOnlyDictionary<string, DeviceSlot> DeviceSlots { get { return _deviceSlots; } }
        
        protected Dictionary<string, Device> _devices = new Dictionary<string, Device>();
        /// <summary>
        /// A dictionary of the active devices currently associated with this tool's device slots, where the key for each item is the name of the device slot.
        /// </summary>
        public IReadOnlyDictionary<string, Device> Devices { get { return _devices; } }


        protected Dictionary<string, DeviceSlotEntry> _lastSetDevices = new Dictionary<string, DeviceSlotEntry>();
        /// <summary>
        /// A dictionary of the devices that were most recently associated with this tool's device slots (either in the current session or past sessions), where the key for each item is the name of the device slot.
        /// This is useful for informing the user, for example, that a tool was most recently paired to a certain device -- e.g., for Paintbrush's Tracked Device, "Last set to Excalibur".
        /// This structure can also be used to automatically set Excalibur to pair with Paintbrush's device slot, whenever Excalibur may happen to connect again (provided a new pairing hasn't been manually performed).
        /// </summary>
        public IReadOnlyDictionary<string, DeviceSlotEntry> LastSetDevices { get { return _lastSetDevices; } }

        public bool logInputToMetricLog = false;

        public Action<Tool> OnDevicesChanged;

        private bool applicationQuitting = false;

        public virtual void OnEnable()
        {
            DeviceManager.DeviceAdded += CheckIfDeviceIsInLastSetDevices;
        }

        public virtual void OnDisable()
        {
            DeviceManager.DeviceAdded -= CheckIfDeviceIsInLastSetDevices;
        }

        public void Start()
        {
            GetLastSetDevices();
            CheckIfAllDevicesAreInLastSetDevices();

            ToolManager.AddTool(this);
        }

        public virtual void OnDestroy()
        {
            if (!applicationQuitting)
                ToolManager.RemoveTool(this);
        }

        public virtual void OnApplicationQuit()
        {
            applicationQuitting = true;
        }

        protected string _name;
        public string Name { get { return _name; } }

        public string persistenceUid;
        /// <summary>
        /// An optional unique identifier used by the ToolManager to persistently keep track of this tool's device pairings. If not specified, uses the name of the tool's Game Object, so this really
        /// only needs to be set if the project is going to have distinct tools that need to be kept track of separately, but those distinct tools' Game Objects have the same name.
        /// </summary>
        public string PersistenceUid
        {
            get
            {
                string toRet;

                if (persistenceUid != "")
                    toRet = persistenceUid;
                else toRet = name;

                return toRet;
            }
        }

        /// <summary>
        /// Sets a device to one of this tool's device slots.
        /// </summary>
        /// <param name="slotName">The name of the device slot to be set to.</param>
        /// <param name="device">The device that is being set.</param>
        public void SetDevice(string slotName, Device device)
        {
            _devices[slotName] = device;
            
            OnDevicesChanged?.Invoke(this);
        }

        /// <summary>
        /// Given the name of a device slot, returns a label for that device slot--
        /// if a device is connected, the display name of that device.
        /// If a device isn't connected, the name of the device this slot was last set to.
        /// If the slot has never been set, the name of the slot.
        /// </summary>
        public string GetLabelForSlot(string slotName)
        {
            string toRet = Devices.ContainsKey(slotName) ?
                Devices[slotName].DisplayName :
                LastSetDevices.ContainsKey(slotName) ?
                    $"Last set to {LastSetDevices[slotName].DeviceDisplayName}" :
                    slotName;

            return toRet;
        }

        public void GetLastSetDevices()
        {
            if (PersistenceMap.ContainsKey(PersistenceUid))
            {
                foreach (KeyValuePair<string, DeviceSlotEntry> kvp in PersistenceMap[PersistenceUid])
                {
                    _lastSetDevices[kvp.Key] = kvp.Value;
                }
            }
        }

        public void CheckIfDeviceIsInLastSetDevices(Device device)
        {
            foreach(KeyValuePair<string, DeviceSlotEntry> kvp in LastSetDevices)
            {
                if(kvp.Value.DeviceUid == device.Uid)
                {
                    SetDevice(kvp.Key, device);
                }
            }
        }

        private void CheckIfAllDevicesAreInLastSetDevices()
        {
            foreach(Device device in DeviceManager.ActiveDevices)
            {
                CheckIfDeviceIsInLastSetDevices(device);
            }
        }

        /// <summary>
        /// <para>Sets the position of the tool while also taking the global pivot into account.</para>
        /// <para>For this reason, it's strongly recommended to use this method when setting this tool's position, rather than modifying the transform directly.</para>
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            //transform.localPosition = PivotManagement.PivotManager.GlobalPivotPos + position;
            transform.localPosition = position;
        }

        /// <summary>
        /// <para>Sets the rotation of the tool while also taking the global pivot into account.</para>
        /// <para>For this reason, it's strongly recommended to use this method when setting this tool's rotation, rather than modifying the transform directly.</para>
        /// </summary>
        public void SetRotation(Quaternion rotation)
        {
            //transform.rotation = PivotManagement.PivotManager.GlobalPivotRot * rotation;
            transform.localRotation = rotation;
        }
    }
}
