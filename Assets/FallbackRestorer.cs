using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FallbackRestorer : MonoBehaviour
{
    public Vector3[] pos = new Vector3[6];
    public Quaternion[] rot = new Quaternion[6];
    public Vector3[] scale = new Vector3[6];

    public bool save;
    public bool restore;

    private void Update()
    {
        if (save)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                pos[i] = transform.GetChild(i).GetChild(0).localPosition;
                rot[i] = transform.GetChild(i).GetChild(0).localRotation;
                scale[i] = transform.GetChild(i).GetChild(0).localScale;
            }
            save = false;
        }

        if(restore)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform toRestore = transform.GetChild(i).GetChild(0);
                toRestore.localPosition = pos[i];
                toRestore.localRotation = rot[i];
                toRestore.localScale = scale[i];
            }
            restore = false;
        }
    }
}
