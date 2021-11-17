using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices.Hands;
using Leap.Unity;
using NextGen.VrManager.Devices.LeapMotion;

namespace NextGen.Tools
{
    public class SnapHand : Tool
    {
        [SerializeField]
        private Handedness handedness = Handedness.None;

        [SerializeField]
        private GameObject renderHandLeft;
        [SerializeField]
        private GameObject renderHandRight;

        private SkinnedMeshRenderer[] allSkinnedMeshRenderers;
        [SerializeField]
        public SkinnedMeshRenderer meshRendererLeft;
        [SerializeField]
        public SkinnedMeshRenderer meshRendererRight;

        private CcdFingerRig ikHandLeft;
        private CcdFingerRig ikHandRight;

        private RiggedHand riggedHandLeft;
        private RiggedHand riggedHandRight;

        [SerializeField]
        private HandTargets handTargets = new HandTargets();

        private float desiredRootWeight;
        private float smoothDampCurrentVelocityRoot;
        private Dictionary<HandFinger, float> desiredWeights = new Dictionary<HandFinger, float>();
        private Dictionary<HandFinger, float> smoothDampCurrentVelocities = new Dictionary<HandFinger, float>();

        [SerializeField]
        [Range(0, 1)]
        private float surfaceMaxUpWeight = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float surfaceMaxDownWeight = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float globalRotationWeight = 1f;

        [SerializeField]
        [Range(0, 1)]
        private float fingerWeightSmoothTime = .5f;
        [SerializeField]
        [Range(0, .5f)]
        private float fingerTargetPositionSmoothTime = .05f;

        [SerializeField]
        private float gravitationCastDownDistance = .01f, gravitationCastUpDistance = .01f;

        [SerializeField]
        private float surfacePositionOffset = -.01f;

        [SerializeField]
        private LayerMask surfaceDetectionLayers;
        [SerializeField]
        private LayerMask snapPointDetectionLayers;

        private bool lostTracking;
        private void Awake()
        {
            _name = "SnapHand";

            _deviceSlots =
            new Dictionary<string, DeviceSlot>() {
                {
                    "Hand",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.HandTracking,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)LeapDeviceFeatures.handData
                        }
                    }
                }
            };

            riggedHandLeft = renderHandLeft.GetComponent<RiggedHand>();
            riggedHandRight = renderHandRight.GetComponent<RiggedHand>();

            ikHandLeft = renderHandLeft.GetComponentInChildren<CcdFingerRig>();
            ikHandRight = renderHandRight.GetComponentInChildren<CcdFingerRig>();

            allSkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            //make invisible
            allSkinnedMeshRenderers.ForEach((SkinnedMeshRenderer smr) => { smr.enabled = false; });
        }

        private void LateUpdate()
        {
            if(Devices.ContainsKey("Hand"))
            {
                //apply pose data to hand, if present                
                if (Devices["Hand"].TryGetFeatureValue(LeapDeviceFeatures.handData, out Leap.Hand leapHand) && leapHand != null
                    && Devices["Hand"].TryGetFeatureValue(CommonDeviceFeatures.handData, out Hand hand) && hand != null)
                {
                    handedness = Devices["Hand"].Characteristics.HasFlag(DeviceCharacteristics.Left) ? Handedness.Left : Handedness.Right;

                    if(handedness == Handedness.None)
                    {
                        return;
                    }

                    CcdFingerRig ikHand = null;
                    RiggedHand riggedHand = null;

                    if (handedness == Handedness.Left)
                    {
                        meshRendererLeft.enabled = true;
                        ikHand = ikHandLeft;
                        riggedHand = riggedHandLeft;
                    }
                    else if (handedness == Handedness.Right)
                    {
                        meshRendererRight.enabled = true;
                        ikHand = ikHandRight;
                        riggedHand = riggedHandRight;
                    }

                    var palmPosition = leapHand.PalmPosition.ToVector3();

                    //Vector3 wristPosition = default;

                    //if (hand.TryGetRootBone(out Bone boneOut))
                    //{
                    //    boneOut.TryGetPosition(out wristPosition);
                    //}

                    SnapHandSnapZone snapPoint = null;

                    foreach (Collider c in Physics.OverlapSphere(palmPosition, 0.01f, snapPointDetectionLayers))
                    {
                        var sp = c.GetComponent<SnapHandSnapZone>();
                        if(sp)
                        {
                            if (sp.handedness == handedness)
                            {
                                snapPoint = sp;
                                break;
                            }
                        }
                    }

                    handTargets.SetPositionsAndRotations(hand, lostTracking);
                    //ikHand.transform.SetPositionAndRotation(handTargets.root.position, handTargets.root.rotation);

                    riggedHand.SetLeapHand(leapHand);

                    if(!snapPoint)
                    {
                        riggedHand.UpdateHand();
                    }

                    if (snapPoint)
                    {
                        handTargets.SetPositionsAndRotations(snapPoint.handTargets, lostTracking);

                        float innerRadius = snapPoint.innerCollider.radius * snapPoint.transform.lossyScale.x;
                        float outerRadius = snapPoint.outerCollider.radius * snapPoint.transform.lossyScale.x;

                        //TODO: look into this more, I'm still a little sketched out as to whether or not this is producing the correct values.
                        float distanceToCenter = Math.Abs(Vector3.Distance(palmPosition, snapPoint.transform.position));
                        float distanceToInner = Mathf.Clamp(distanceToCenter - innerRadius, 0, float.MaxValue);
                        float innerOuterDiff = Math.Abs(outerRadius - innerRadius);
                        float weightToSet = 1;

                        snapPoint.lastDistToCenter = distanceToCenter;
                        snapPoint.lastDistToInner = distanceToInner;
                        snapPoint.lastWeightDerived = weightToSet;

                        if(snapPoint.handTargets.root.enabled)
                            desiredRootWeight = weightToSet;

                        //ikHand.transform.SetPositionAndRotation(handTargets.root.position, handTargets.root.rotation);
                        foreach (HandFinger handFinger in Enum.GetValues(typeof(HandFinger)))
                        {
                            HandTargets handTargets = snapPoint.handTargets;
                            FingerTarget fingerTarget = handTargets.GetTargetForFinger(handFinger);
                            if (fingerTarget.enabled)
                                desiredWeights[handFinger] = weightToSet;
                            else
                                GravitateFingerToSurface(hand, leapHand.PalmarAxis(), handFinger);
                        }
                    }
                    else
                    {
                        desiredRootWeight = 0;

                        foreach (HandFinger handFinger in Enum.GetValues(typeof(HandFinger)))
                        {
                            desiredWeights[handFinger] = 0;
                            FingerTarget fingerTarget = handTargets.GetTargetForFinger(handFinger);

                            var bones = new List<Bone>();
                            hand.TryGetFingerBones(handFinger, bones);
                            bones.Last().TryGetPosition(out Vector3 position);

                            SnapHandSnapFingerZone snapFingerZone = null;

                            foreach (Collider c in Physics.OverlapSphere(position, 0.01f, snapPointDetectionLayers))
                            {
                                var sp = c.GetComponent<SnapHandSnapFingerZone>();
                                if (sp)
                                {
                                    if (sp.handedness == handedness && sp.fingerType == handFinger && sp.fingerTarget.enabled)
                                    {
                                        snapFingerZone = sp;
                                        break;
                                    }
                                }
                            }

                            if(snapFingerZone)
                            {
                                float distanceToCenter = Math.Abs(Vector3.Distance(position, snapFingerZone.transform.position));
                                float distanceToInner = Mathf.Clamp(distanceToCenter - snapFingerZone.innerCollider.radius, 0, float.MaxValue);
                                float innerOuterDiff = Math.Abs(snapFingerZone.outerCollider.radius - snapFingerZone.innerCollider.radius);
                                float weightToSet = 1;

                                desiredWeights[handFinger] = weightToSet;
                                fingerTarget.SetPositionAndRotation(snapFingerZone.fingerTarget);
                            }
                            else
                            {
                                GravitateFingerToSurface(hand, leapHand.PalmarAxis(), handFinger);
                            }
                        }
                    }

                    LerpFingerWeights(ikHand);
                    handTargets.UpdateTargetPositions(fingerTargetPositionSmoothTime);
                    lostTracking = false;
                }
                else
                {
                    lostTracking = true;
                    //make invisible
                    allSkinnedMeshRenderers.ForEach((SkinnedMeshRenderer smr) => { smr.enabled = false; });
                }
            }
            else //try to automatically set the device
            {
                Device device = null;

                if (handedness != Handedness.None)
                {
                    device = VrManager.Devices.DeviceManager.GetDeviceByUID($"Leap Motion {(handedness == Handedness.Left ? "Left" : "Right")} Hand");
                }

                if (device != null)
                {
                    SetDevice("Hand", device);
                }
            }
        }

        private void LerpFingerWeights(CcdFingerRig ikHand)
        {
            var root = ikHand.root;
            root.weight = Mathf.SmoothDamp(root.weight, desiredRootWeight, ref smoothDampCurrentVelocityRoot, fingerWeightSmoothTime);
            //root.solver.IKRotationWeight = root.solver.IKPositionWeight;

            var weightKeys = desiredWeights.Keys.ToList();
            foreach (HandFinger handFinger in weightKeys)
            {
                var finger = ikHand.fingers[handFinger];
                if(!smoothDampCurrentVelocities.ContainsKey(handFinger))
                {
                    smoothDampCurrentVelocities[handFinger] = 0;
                }
                var currentVelocity = smoothDampCurrentVelocities[handFinger];
                finger.solver.IKPositionWeight = Mathf.SmoothDamp(finger.solver.IKPositionWeight, desiredWeights[handFinger], ref currentVelocity, fingerWeightSmoothTime);
                smoothDampCurrentVelocities[handFinger] = currentVelocity;
                //finger.rotationWeight = globalRotationWeight * finger.weight;
            }
        }

        public HandFinger GetHandFingerForFinger(Leap.Finger finger)
        {
            return finger.Type switch
            {
                Leap.Finger.FingerType.TYPE_INDEX => HandFinger.Index,
                Leap.Finger.FingerType.TYPE_MIDDLE => HandFinger.Middle,
                Leap.Finger.FingerType.TYPE_RING => HandFinger.Ring,
                Leap.Finger.FingerType.TYPE_PINKY => HandFinger.Pinky,
                _ => HandFinger.Thumb,
            };
        }

        public enum Handedness
        {
            None,
            Left,
            Right
        }

        public void GravitateAllFingersToSurface(Hand nextGenHand, Vector3 direction)
        {
            if (nextGenHand != null)
            {
                foreach (HandFinger handFinger in Enum.GetValues(typeof(HandFinger)))
                {
                    GravitateFingerToSurface(nextGenHand, direction, handFinger);
                }
            }
        }

        public void GravitateFingerToSurface(Hand nextGenHand, Vector3 direction, HandFinger handFinger)
        {
            direction.Normalize();
            Vector3 upDirection = direction * gravitationCastUpDistance;
            Vector3 downDirection = direction * gravitationCastDownDistance;
            Vector3 totalDirection = direction * (gravitationCastUpDistance + gravitationCastDownDistance);
            FingerTarget target = handTargets.GetTargetForFinger(handFinger);
            var bones = new List<Bone>();
            nextGenHand.TryGetFingerBones(handFinger, bones);

            bones.Last().TryGetPosition(out Vector3 position);

            Ray gravitationRay = new Ray(position - upDirection, totalDirection);
            Debug.DrawLine(gravitationRay.origin, gravitationRay.origin + upDirection, Color.yellow);
            Debug.DrawLine(gravitationRay.origin + upDirection, gravitationRay.origin + totalDirection, Color.green);
            if (Physics.Raycast(gravitationRay, out RaycastHit hitInfo, totalDirection.magnitude, surfaceDetectionLayers))
            {
                Debug.DrawLine(gravitationRay.origin, hitInfo.point, Color.red);

                target.SetPositionAndRotation(hitInfo.point + direction * surfacePositionOffset, Quaternion.Euler(hitInfo.normal));
                if(hitInfo.distance < gravitationCastUpDistance)
                {
                    desiredWeights[handFinger] = Mathf.Clamp((hitInfo.distance / gravitationCastUpDistance) * surfaceMaxUpWeight, 0, surfaceMaxUpWeight);
                }
                else
                {
                    var distanceFromFinger = hitInfo.distance - gravitationCastUpDistance;
                    desiredWeights[handFinger] = Mathf.Clamp((1 - (distanceFromFinger / gravitationCastDownDistance)) * surfaceMaxDownWeight, 0, surfaceMaxDownWeight);
                }
            }
        }

        [Serializable]
        public class HandTargets
        {
            public Dictionary<HandFinger, FingerTarget> FingerTransforms { get { return new Dictionary<HandFinger, FingerTarget>() { { HandFinger.Index, index }, { HandFinger.Middle, middle }, { HandFinger.Ring, ring }, { HandFinger.Pinky, pinky }, { HandFinger.Thumb, thumb } }; } }
            public FingerTarget root;
            public FingerTarget index, middle, ring, pinky, thumb = new FingerTarget();

            public void SetPositionsAndRotations(Hand nextGenHand, bool immediate = false)
            {
                if (nextGenHand != null)
                {
                    nextGenHand.TryGetRootBone(out Bone bone);

                    bone.TryGetPosition(out Vector3 rootPosition);
                    bone.TryGetRotation(out Quaternion rootRotation);
                    root.SetPositionAndRotation(rootPosition, rootRotation);

                    foreach (HandFinger handFinger in Enum.GetValues(typeof(HandFinger)))
                    {
                        FingerTarget target = GetTargetForFinger(handFinger);
                        var bones = new List<Bone>();
                        nextGenHand.TryGetFingerBones(handFinger, bones);

                        bones.Last().TryGetPosition(out Vector3 position);
                        bones.Last().TryGetRotation(out Quaternion rotation);
                        target.SetPositionAndRotation(position, rotation, immediate);
                    }
                }
            }

            public void SetPositionsAndRotations(HandTargets other, bool immediate = false)
            {
                if (other != null)
                {
                    root.SetPositionAndRotation(other.root);

                    foreach(HandFinger hf in FingerTransforms.Keys)
                    {
                        var target = FingerTransforms[hf];
                        var otherTarget = other.FingerTransforms[hf];
                        if(otherTarget.target != null)
                        {
                            target.SetPositionAndRotation(otherTarget, immediate);
                        }
                    }
                }
            }

            public FingerTarget GetTargetForFinger(HandFinger hf)
            {
                switch(hf)
                {
                    case HandFinger.Index:
                        return index;
                    case HandFinger.Middle:
                        return middle;
                    case HandFinger.Ring:
                        return ring;
                    case HandFinger.Pinky:
                        return pinky;
                    case HandFinger.Thumb:
                    default:
                        return thumb;
                }
            }

            public void UpdateTargetPositions(float fingerTargetPositionSmoothTime)
            {
                root.UpdateTargetPositionAndRotation(fingerTargetPositionSmoothTime);

                foreach(FingerTarget target in FingerTransforms.Values)
                {
                    target.UpdateTargetPositionAndRotation(fingerTargetPositionSmoothTime);
                }
            }
        }
        
        [Serializable]
        public class FingerTarget
        {
            public bool enabled;
            public Transform target;
            private Vector3 desiredPosition;
            private Quaternion desiredRotation;
            private Vector3 currentVelocity;
            private Quaternion derivRot;

            public void SetPositionAndRotation(FingerTarget other, bool immediate = false)
            {
                SetPositionAndRotation(other.target.position, other.target.rotation, immediate);
            }

            public void SetPositionAndRotation(Vector3 position, Quaternion rotation, bool immediate = false)
            {
                desiredPosition = position;
                desiredRotation = rotation;
                if(immediate)
                {
                    target.position = desiredPosition;
                    target.rotation = desiredRotation;
                }
            }

            public void UpdateTargetPositionAndRotation(float fingerTargetPositionSmoothTime)
            {
                target.position = Vector3.SmoothDamp(target.position, desiredPosition, ref currentVelocity, fingerTargetPositionSmoothTime);
                target.rotation = SmoothDamp(target.rotation, desiredRotation, ref derivRot, fingerTargetPositionSmoothTime);
            }
        }

        /// <summary>
        /// https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
        /// </summary>
        /// <param name="rot"></param>
        /// <param name="target"></param>
        /// <param name="deriv"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
        {
            if (Time.deltaTime < Mathf.Epsilon) return rot;
            // account for double-cover
            var Dot = Quaternion.Dot(rot, target);
            var Multi = Dot > 0f ? 1f : -1f;
            target.x *= Multi;
            target.y *= Multi;
            target.z *= Multi;
            target.w *= Multi;
            // smooth damp (nlerp approx)
            var Result = new Vector4(
                Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
                Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
                Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
                Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
            ).normalized;

            // ensure deriv is tangent
            var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
            deriv.x -= derivError.x;
            deriv.y -= derivError.y;
            deriv.z -= derivError.z;
            deriv.w -= derivError.w;

            return new Quaternion(Result.x, Result.y, Result.z, Result.w);
        }
    }
}