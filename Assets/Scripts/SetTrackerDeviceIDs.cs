// SetTrackerDeviceIDs.cs
// 
// Set SteamVR tracker IDs in a single location so we don't have to search through the scene hierarchy to find and change tracker IDs.
// Functionality is the same as setting device IDs on each individual SteamVR_TrackedObject game object, but having values in a single location enables fast configuration
//
// Created by Jason Jerald as suggested by Jason Haskins on April 23, 2019


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetTrackerDeviceIDs : MonoBehaviour
{
    [System.Serializable]

    // Tracker class that contains a tracker and ID information
    // This would better be a struct as it only contains data but foreach code in SetTrackerDeviceIDs methods won't compile if a struct
    public class Tracker
    {
        public SteamVR_TrackedObject trackedObject;
        public SteamVR_TrackedObject.EIndex deviceID;
        internal SteamVR_TrackedObject.EIndex deviceIDPrevious;
    }

    // Array of Trackers
    public Tracker [] trackers;

    public bool loadOnStart = true;

    public bool loadAutomatically;



    void Start()
    {
        if (loadOnStart)
        {
            loadIDs();
        }

        // Initialize previous IDs to current IDs
        foreach (Tracker tracker in trackers)
        {
            if (tracker.trackedObject != null)
            {
                tracker.trackedObject.SetDeviceIndex((int)tracker.deviceID);
                tracker.deviceIDPrevious = tracker.deviceID;
            }
        }
    }


    void Update()
    {
        if(Input.GetKey(KeyCode.L))
        { 
            loadIDs();
        }

        if (Input.GetKey(KeyCode.S))
        {
            saveIDs();
        }

        // If a device ID changed then update the referenced tracker with the new ID
        foreach (Tracker tracker in trackers)
        {
            if(tracker.trackedObject != null)
            {
                tracker.trackedObject.SetDeviceIndex((int)tracker.deviceID);
                tracker.deviceIDPrevious = tracker.deviceID;
            }
        }
    }


    public void saveIDs()
    {

        ETrackedDeviceProperty prop = ETrackedDeviceProperty.Prop_SerialNumber_String;
        var error = ETrackedPropertyError.TrackedProp_Success;
        for(int i = 0; i < trackers.Length; i++)
        {
            var result = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty((uint)trackers[i].deviceID, prop, result, 64, ref error);

            //Debug.Log(result);

            PlayerPrefs.SetString("TrackerIdnum" + i, result.ToString());
        }

    }

    public void loadIDs()
    {
        ETrackedDeviceProperty prop = ETrackedDeviceProperty.Prop_SerialNumber_String;
        var error = ETrackedPropertyError.TrackedProp_Success;
        for(int i = 0; i < trackers.Length; i++)
        {
            string serialNum = PlayerPrefs.GetString("TrackerIdnum" + i);

            for (uint index = 0; index < 16; index++)
            {
                var result = new System.Text.StringBuilder((int)64);
                OpenVR.System.GetStringTrackedDeviceProperty(index, prop, result, 64, ref error);

                //Debug.Log(result + " " + serialNum);
                if (result.ToString().Contains(serialNum))
                {
                    trackers[i].deviceID = (SteamVR_TrackedObject.EIndex)index;
                }
            }
        }
    }




#if UNITY_EDITOR

    [CustomEditor(typeof(SetTrackerDeviceIDs))]
    public class SetTrackerDeviceIDsEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (EditorApplication.isPlaying)
            {

                if (GUILayout.Button("save"))
                {
                    ((SetTrackerDeviceIDs)(target)).saveIDs();
                }
                if (GUILayout.Button("load"))
                {
                    ((SetTrackerDeviceIDs)(target)).loadIDs();
                }
            }
        }
    }
#endif

}
