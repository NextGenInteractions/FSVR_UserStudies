using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SteamVRDeviceData : SteamVR_TrackedObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnNewPoses(TrackedDevicePose_t[] poses)
    {
        if (index == EIndex.None)
            return;

        var i = (int)index;

        isValid = false;
        if (poses.Length <= i)
            return;

        if (!poses[i].bDeviceIsConnected)
            return;

        if (!poses[i].bPoseIsValid)
            return;

        isValid = true;

        var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
        var pos = DataFilterManager.trackingInstance.FilterPositionDeviceIndex(this, pose.pos);
        if (origin != null)
        {
            transform.position = origin.transform.TransformPoint(pos);
            transform.rotation = origin.rotation * pose.rot;
        }
        else
        {
            transform.localPosition = pos;
            transform.localRotation = pose.rot;
        }
    }
}
