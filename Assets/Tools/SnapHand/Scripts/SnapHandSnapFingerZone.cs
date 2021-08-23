using NextGen.VrManager.Devices.Hands;
using UnityEngine;

namespace NextGen.Tools
{
    public class SnapHandSnapFingerZone : MonoBehaviour
    {
        public SnapHand.Handedness handedness;

        [Header("Finger")]
        public HandFinger fingerType;
        public SnapHand.FingerTarget fingerTarget;

        [Header("Field Colliders")]
        public SphereCollider innerCollider; //TODO: make this work with any type of collider, not just spheres
        public SphereCollider outerCollider; //TODO: make this work with any type of collider, not just spheres
    }
}