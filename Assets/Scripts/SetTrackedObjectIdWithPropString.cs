using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetTrackedObjectIdWithPropString : MonoBehaviour
{

    SteamVR_TrackedObject obj;

    public ETrackedDeviceProperty prop = ETrackedDeviceProperty.Prop_RenderModelName_String;

    public string searchString;

    bool isSet = false;

    // Start is called before the first frame update
    void Start()
    {
        setId();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown("m"))
        {
            setId();
        }
        //if (!isSet)
        {
            //setId();
        }
    }
    public void setId()
    {
        int index = -1; // -1 is None, 0 is HMD
        var error = ETrackedPropertyError.TrackedProp_Success;

        for (int i = 0; i < 16; i++)
        {
            var result = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty((uint)i, prop, result, 64, ref error);


            Debug.Log(result);

            if (result.ToString().Contains(searchString))
            {
                index = i;
                isSet = true;
                break;
            }
        }

        obj = GetComponent<SteamVR_TrackedObject>();
        obj.index = (SteamVR_TrackedObject.EIndex)index;
    }

#if UNITY_EDITOR

	[CustomEditor( typeof( SetTrackedObjectIdWithPropString) )]
	public class SetTrackedObjectidEditor : Editor
    {
         
		public override void OnInspectorGUI()
        {
			DrawDefaultInspector();

            if (GUILayout.Button("Retry"))
            {
                ((SetTrackedObjectIdWithPropString)(target)).setId();
            }


        }
    }
#endif
}
