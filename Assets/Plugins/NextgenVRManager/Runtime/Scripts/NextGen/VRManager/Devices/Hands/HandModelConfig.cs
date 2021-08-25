using NextGen.VrManager.Devices.Hands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandModelConfig : MonoBehaviour
{
    [SerializeField]
    public SkinnedMeshRenderer handRenderer;
    [SerializeField]
    public Transform transformToFlip;
    [SerializeField]
    public Vector3 leftScale = new Vector3(-1, 1, 1);
    [SerializeField]
    public Vector3 rightScale = new Vector3(1, 1, 1);

    [SerializeField]
    public Transform rootBone;
    [SerializeField]
    public Vector3 rootRotation;
    [SerializeField]
    public Transform[]
        indexBones = new Transform[4],
        middleBones = new Transform[4],
        ringBones = new Transform[4],
        pinkyBones = new Transform[4],
        thumbBones = new Transform[4];
    [SerializeField]
    public Vector3[]
        indexRotations = new Vector3[4],
        middleRotations = new Vector3[4],
        ringRotations = new Vector3[4],
        pinkyRotations = new Vector3[4],
        thumbRotations = new Vector3[4];

    public Transform[] GetTransforms(HandFinger hf)
    {
        switch (hf)
        {
            case HandFinger.Index:
                return indexBones;
            case HandFinger.Middle:
                return middleBones;
            case HandFinger.Ring:
                return ringBones;
            case HandFinger.Pinky:
                return pinkyBones;
            case HandFinger.Thumb:
            default:
                return thumbBones;
        }
    }

    public Vector3[] GetRotations(HandFinger hf)
    {
        switch (hf)
        {
            case HandFinger.Index:
                return indexRotations;
            case HandFinger.Middle:
                return middleRotations;
            case HandFinger.Ring:
                return ringRotations;
            case HandFinger.Pinky:
                return pinkyRotations;
            case HandFinger.Thumb:
            default:
                return thumbRotations;
        }
    }
}
