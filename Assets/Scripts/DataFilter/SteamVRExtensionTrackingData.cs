using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SteamVRExtensionTrackingData : SteamVR_Behaviour_Pose
{
    public enum TrackingType
    {
        Role,
        Index,
        Tag
    }

    [HideInInspector]
    public TrackingType type = TrackingType.Index;

    private List<Vector3> lostTrackingDetectionList = new List<Vector3>();
    private int lostTrackingThreshold = 10; // for role type to use for detecting lost-tracking
    private bool _isLostTracking = false;
    private int frameCounter = 0; // index&tag type to use for detecting lost-tracking

    public bool isLostTracking { get => _isLostTracking; }
    public bool isDetectOutliner { get => DataFilterManager.trackingInstance.IsDetectOutliner(this); }
    public int tolerancePoint { get => DataFilterManager.trackingInstance.TolerancePoint(this); }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // InvokeRepeating("GetBatteryLevel", 5, 1);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (type != TrackingType.Role)
            _isLostTracking = ++frameCounter > lostTrackingThreshold;
    }

    void AddPoseForDetection(Vector3 pose)
    {
        if (lostTrackingDetectionList.Count >= lostTrackingThreshold)
        {
            lostTrackingDetectionList.Add(pose);
            lostTrackingDetectionList.RemoveAt(0);
            int index = lostTrackingDetectionList.FindIndex(1, lostTrackingThreshold - 1, a => !Vector3.Equals(a, lostTrackingDetectionList[0]));
            _isLostTracking = index < 0;
        }
        else
            lostTrackingDetectionList.Add(pose);
    }

    protected override void UpdateTransform()
    {
        AddPoseForDetection(poseAction[inputSource].localPosition);
        Vector3 filteredPosition = DataFilterManager.trackingInstance.FilterPositionPose(this, poseAction[inputSource].localPosition);
        CheckDeviceIndex();

        if (origin != null)
        {
            transform.position = origin.transform.TransformPoint(filteredPosition);
            transform.rotation = origin.rotation * poseAction[inputSource].localRotation;
        }
        else
        {
            transform.localPosition = filteredPosition;
            transform.localRotation = poseAction[inputSource].localRotation;
        }
    }
    public float GetBatteryLevel()
    {
        uint index = (type == TrackingType.Role)? (uint)GetDeviceIndex() : (uint)this.index;
        if (!OpenVR.System.IsTrackedDeviceConnected(index))
            return -1;
        ETrackedDeviceProperty prop = ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float;
        var error = ETrackedPropertyError.TrackedProp_Success;
        float result = OpenVR.System.GetFloatTrackedDeviceProperty(index, prop, ref error);
        return result;
    }

    #region Index_Tracking
    public enum EIndex
    {
        None = -1,
        Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
        Device1,
        Device2,
        Device3,
        Device4,
        Device5,
        Device6,
        Device7,
        Device8,
        Device9,
        Device10,
        Device11,
        Device12,
        Device13,
        Device14,
        Device15,
        Device16
    }

    [HideInInspector]
    public EIndex index = EIndex.Hmd;

    public bool isValid_Index { get; protected set; }

    protected virtual void OnNewPoses(TrackedDevicePose_t[] poses)
    {
        if (index == EIndex.None)
            return;

        var i = (int)index;

        isValid_Index = false;
        if (poses.Length <= i)
            return;

        if (!poses[i].bDeviceIsConnected)
            return;

        if (!poses[i].bPoseIsValid)
            return;

        isValid_Index = true;
        frameCounter = 0;

        var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
        var pos = DataFilterManager.trackingInstance.FilterPositionPose(this, pose.pos);
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

    SteamVR_Events.Action newPosesAction;

    protected SteamVRExtensionTrackingData()
    {
        if (type != TrackingType.Role)
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
    }

    protected void Awake()
    {
        OnEnable();
    }

    protected override void OnEnable()
    {
        if (type != TrackingType.Role)
        {
            var render = SteamVR_Render.instance;
            if (render == null)
            {
                enabled = false;
                return;
            }

            newPosesAction.enabled = true;
            if (type == TrackingType.Tag)
                setId();
        }
        else
            base.OnEnable();
    }

    protected override void OnDisable()
    {

        if (type != TrackingType.Role)
        {
            newPosesAction.enabled = false;
            isValid_Index = false;
        }
        else
            base.OnDisable();
    }

    public void SetDeviceIndex(int index)
    {
        if (System.Enum.IsDefined(typeof(EIndex), index))
            this.index = (EIndex)index;
    }
    #endregion

    #region Tag_Tracking

    [HideInInspector]
    public ETrackedDeviceProperty prop = ETrackedDeviceProperty.Prop_RenderModelName_String;

    [HideInInspector]
    public string searchString = "";
    bool isSet = false;

    public int setId()
    {
        if (searchString.Length <= 0)
            return -1;
        int index = -1; // -1 is None, 0 is HMD
        var error = ETrackedPropertyError.TrackedProp_Success;

        for (int i = 0; i < 16; i++)
        {
            if (OpenVR.System.IsTrackedDeviceConnected((uint)i))
            {
                var result = new System.Text.StringBuilder((int)64);
                OpenVR.System.GetStringTrackedDeviceProperty((uint)i, prop, result, 64, ref error);


                if (result.ToString().Contains(searchString))
                {
                    index = i;
                    isSet = true;
                    break;
                }
            }

        }

        this.index = (EIndex)index;
        return index;
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(SteamVRExtensionTrackingData))]
public class SteamVRTrackingDataEditor: Editor
{

    override public void OnInspectorGUI()
    {
        SteamVRExtensionTrackingData instance = (SteamVRExtensionTrackingData)target;
        if (instance.type == SteamVRExtensionTrackingData.TrackingType.Role)
        {
            instance.type = (SteamVRExtensionTrackingData.TrackingType)EditorGUILayout.EnumPopup("Type", instance.type);
            DrawDefaultInspector();
        }
        else if (instance.type == SteamVRExtensionTrackingData.TrackingType.Index)
        {
            instance.type = (SteamVRExtensionTrackingData.TrackingType)EditorGUILayout.EnumPopup("Type", instance.type);

            instance.index = (SteamVRExtensionTrackingData.EIndex) EditorGUILayout.EnumPopup("Device Index", instance.index);

            instance.origin = EditorGUILayout.ObjectField("Transform Origin", instance.origin, typeof(Transform), true) as Transform;
        }
        else
        {
            instance.type = (SteamVRExtensionTrackingData.TrackingType) EditorGUILayout.EnumPopup("Type", instance.type);

            instance.origin = EditorGUILayout.ObjectField("Transform Origin", instance.origin, typeof(Transform), true) as Transform;

            instance.prop = (ETrackedDeviceProperty)EditorGUILayout.EnumPopup("Device Property String", instance.prop);

            string str = instance.searchString;
            instance.searchString = EditorGUILayout.TextField("Tag String", instance.searchString);
            if (!str.Equals(instance.searchString))
                instance.setId();

            EditorGUILayout.TextField("Device Index", instance.index.ToString());
        }
    }
}
#endif