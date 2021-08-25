using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using System.Collections.Generic;

namespace NextGen.VrManager.ToolManagement
{
    /// <summary>
    /// ITool represents an object which in some way makes use of one or more devices.
    /// Tools can have "requirements" for which types of devices they need access to to function properly
    /// </summary>
    public interface ITool
    {
        public string Name { get; }
        IReadOnlyDictionary<string, DeviceSlot> DeviceSlots { get; }
        IReadOnlyDictionary<string, Device> Devices { get; }

        public void SetDevice(string slotName, Device device);
    }

    public struct DeviceSlot
    {
        public DeviceCharacteristics RequiredCharacteristics;
        public List<DeviceFeatureUsage> RequiredFeatures;
    }
}