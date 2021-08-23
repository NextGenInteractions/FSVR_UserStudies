using UnityEngine;

namespace NextGen.Tools
{
    public class SnapHandSnapZone : MonoBehaviour
    {
        public SnapHand.Handedness handedness;

        [Header("Hand Targets")]
        public SnapHand.HandTargets handTargets;

        [Header("Field Colliders")]
        public SphereCollider innerCollider; //TODO: make this work with any type of collider, not just spheres
        public SphereCollider outerCollider; //TODO: make this work with any type of collider, not just spheres

        public float lastDistToCenter;
        public float lastDistToInner;
        [Range(0,1)] public float lastWeightDerived;
    }
}