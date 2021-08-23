using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LineRendererToPointFile : MonoBehaviour
{

    public GameObject root;
    public string file;

    public void process()
    {
        LineRenderer[] lines = root.GetComponentsInChildren<LineRenderer>();
        
        StreamWriter writer = new StreamWriter(file);

        for (int i = 0; i < lines.Length; i++)
        {
            writer.WriteLine(getLineString(lines[i]));
        }

        writer.Close();

    }

    string getLineString(LineRenderer line)
    {

        string s = "curve";

        Vector3[] positions = new Vector3[line.positionCount];
         line.GetPositions(positions);

        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 v = positions[i];
            s += " " + v.x + " " + v.y + " " + v.z;
        }

        return s +"\n";
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LineRendererToPointFile))]
    public class lineRentererToPointFileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save"))
            {
                ((LineRendererToPointFile)(target)).process();
            }
        }
    }
#endif
}
