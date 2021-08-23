using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinDeviceIdSetter : MonoBehaviour
{
    public List<SteamVRExtensionTrackingData> trackers = new List<SteamVRExtensionTrackingData>();

    public void SetHeadDeviceId(int id)
    {
        SetDevice(0, id);
    }

    public void SetPelvisDeviceId(int id)
    {
        SetDevice(1, id);
    }

    public void SetLeftArmDeviceId(int id)
    {
        SetDevice(2, id);
    }

    public void SetRightArmDeviceId(int id)
    {
        SetDevice(3, id);
    }

    public void SetLeftLegDeviceId(int id)
    {
        SetDevice(4, id);
    }

    public void SetRightLegDeviceId(int id)
    {
        SetDevice(5, id);
    }

    void SetDevice(int device, int id)
    {
        trackers[device].enabled = true;
        trackers[device].SetDeviceIndex(id);
    }
}
