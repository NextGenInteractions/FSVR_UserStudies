using NextGen.VrManager.Devices.Hands;
using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;

public class CcdFingerRig : MonoBehaviour
{
    [SerializeField]
    public LateMoveTo root;
    public CCDIK index, middle, ring, pinky, thumb;

    public IReadOnlyDictionary<HandFinger, CCDIK> fingers { get { return new Dictionary<HandFinger, CCDIK>() { { HandFinger.Index, index }, { HandFinger.Middle, middle }, { HandFinger.Ring, ring }, { HandFinger.Pinky, pinky }, { HandFinger.Thumb, thumb } }; } }
}
