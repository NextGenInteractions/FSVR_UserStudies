using System.Text;
using Valve.VR;

public struct SteamVrDevice
{
    public bool isValid { get; private set; }

    public string name { get; private set; }

    public string serialNumber { get; private set; }

    public float batteryLevel { get; private set; }

    public SteamVrDevice(uint deviceId)
    {
        ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;

        var nameSb = new StringBuilder(64);
        OpenVR.System.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_RenderModelName_String, nameSb, 64, ref error);
        name = nameSb.ToString();

        isValid = name != "" && OpenVR.System.IsTrackedDeviceConnected(deviceId);

        if(isValid)
        {
            StringBuilder serialNumberSb = new StringBuilder(64);
            OpenVR.System.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_SerialNumber_String, serialNumberSb, 64, ref error);
            serialNumber = serialNumberSb.ToString();

            batteryLevel = OpenVR.System.GetFloatTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float, ref error);
        }
        else
        {
            serialNumber = "";
            batteryLevel = 0;
        }
    }
}
