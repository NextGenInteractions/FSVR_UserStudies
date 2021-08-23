using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NextGen;

//Custom inspector for this component.
[CustomEditor(typeof(DeviceManager))]
public class DeviceManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DeviceManager deviceManager = (DeviceManager)target;
        GUILayout.Label("Config Devices");
        foreach (KeyValuePair<string, string> pair in deviceManager.nametags)
        {
            GUILayout.Label(pair.Key + ":" + pair.Value);
            Repaint();
        }
    }
}