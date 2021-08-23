using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PoseColliderController : MonoBehaviour
{
    public enum ColliderToggleType
    {
        None,
        OppositeSide, // toggle opposite side
        SameSideExclusive, // exclude the one that triggers
        SameSideInclusive, // toggle the same side of the one that triggers including itself
        SiblingExclusive, // toggle the same side of the one that triggers excluding itself
        Other, // all others except the one that triggers
        All, // all including the one that triggers
        Self // only the one that triggers
    }

    public ColliderToggleType toggleType = ColliderToggleType.None;
    [HideInInspector]
    public bool isColliding;
    
    [Tooltip("If not filled, it will try to load from children")]
    public List<ObjectPoseConfiguration> poseScriptList;
    List<Collider> leftColliders = new List<Collider>();
    List<Collider> rightColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        if (poseScriptList == null || poseScriptList.Count <= 0)
            poseScriptList = new List<ObjectPoseConfiguration>(GetComponentsInChildren<ObjectPoseConfiguration>());

        foreach(ObjectPoseConfiguration opf in poseScriptList)
        {
            if (opf.colliderLeft != null && opf.colliderRight != null)
            {
                leftColliders.Add(opf.colliderLeft);
                leftColliders.Add(opf.colliderRight);
                opf.SetColliderController(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Event to toggle other colliders
    /// Triggered by one of the colliders listed
    /// </summary>
    /// <param name="script"></param>
    /// <param name="isLeft"></param>
    public void ColliderEvent (ObjectPoseConfiguration script, bool isLeft)
    {
        List<Collider> sideList = isLeft ? leftColliders : rightColliders;
        Collider collider = isLeft ? script.colliderLeft : script.colliderRight;
        switch (toggleType)
        {
            case ColliderToggleType.Self:
                collider.enabled = false;
                break;

            case ColliderToggleType.SameSideInclusive:
                sideList.ForEach(c => c.enabled = !c.enabled);
                break;

            case ColliderToggleType.SameSideExclusive:
                sideList.ForEach(c => c.enabled = !c.enabled);
                collider.enabled = !collider.enabled;
                break;

            case ColliderToggleType.OppositeSide:
                if (sideList == leftColliders)
                    rightColliders.ForEach(c => c.enabled = !c.enabled);
                else
                    leftColliders.ForEach(c => c.enabled = !c.enabled);
                break;

            case ColliderToggleType.Other:
                leftColliders.ForEach(c => c.enabled = !c.enabled);
                rightColliders.ForEach(c => c.enabled = !c.enabled);
                collider.enabled = !collider.enabled;
                break;

            case ColliderToggleType.All:
                leftColliders.ForEach(c => c.enabled = !c.enabled);
                rightColliders.ForEach(c => c.enabled = !c.enabled);
                break;

            case ColliderToggleType.SiblingExclusive:
                foreach(ObjectPoseConfiguration opf in poseScriptList)
                {
                    if (opf != script)
                    {
                        opf.colliderLeft.enabled = !opf.colliderLeft.enabled;
                        opf.colliderRight.enabled = !opf.colliderRight.enabled;
                    }
                }
                break;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PoseColliderController))]
public class PoseColliderControllerEditor : Editor
{

    override public void OnInspectorGUI()
    {
        PoseColliderController instance = (PoseColliderController)target;

        ObjectPoseConfiguration[] children = instance.transform.GetComponentsInChildren<ObjectPoseConfiguration>();
        var style = new GUIStyle(GUI.skin.label);
        
        style.normal.textColor = Color.red;
        style.fontSize = 20;
        style.wordWrap = true;
        if (children.Length <= 0 && instance.poseScriptList.Count <= 0)
        {
            GUILayout.Label("WARNING", style);
            style.normal.textColor = Color.yellow;
            style.fontSize = 15;
            GUILayout.Label("No <ObjectPoseConfiguration> found in the children!!!", style);
            GUILayout.Label("Please double check that you have attached the script on the right gameobject!", style);
            style.normal.textColor = Color.cyan;
            GUILayout.Label("Or add the scripts to the list", style);
        }
        DrawDefaultInspector();
    }
}
#endif