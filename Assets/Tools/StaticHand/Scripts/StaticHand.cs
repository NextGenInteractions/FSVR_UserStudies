using NextGen.VrManager.Devices;
using NextGen.VrManager.Devices.Hands;
using NextGen.VrManager.PivotManagement;
using NextGen.VrManager.ToolManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticHand : Tool
{
    [SerializeField] public Handedness handedness = Handedness.None;

    private SkinnedMeshRenderer[] allSkinnedMeshRenderers;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private CcdFingerRig ikHand;

    //[SerializeField] private HandTargets handTargets = new HandTargets();
    [SerializeField] private Transform ikTargetPositions;
    [SerializeField] private Transform looseTargetPositions;
    [SerializeField] private Transform grippedTargetPositions;

    private Pivot pivot;

    private float gripState = 0;
    private float thumbState = 0;
    private float timeToFullyGrip = 0.25f;

    private Vector3 lastPos;
    private Quaternion lastRot;

    [HideInInspector] public bool setInvisible = false;

    public bool IsGripping { get { return gripState == 1; } }

    [SerializeField] private float grabRadius = 0.125f;
    [SerializeField] private StaticHandGrabbable grabTarget;

    [SerializeField] private bool alwaysGrip = false;

    public List<StaticHandGrabbableDropZone> allDropZones;
    private bool DropZoneValid
    {
        get
        {
            if (allDropZones.Count == 0)
                return true;
            else
            {
                bool inDropZone = false;

                foreach(Collider col in Physics.OverlapSphere(transform.position, 0.01f))
                {
                    StaticHandGrabbableDropZone zone = col.GetComponent<StaticHandGrabbableDropZone>();
                    if (zone != null)
                        inDropZone = true;
                }

                return inDropZone;
            }
        }
    }

    /*
    private float desiredRootWeight;
    private float smoothDampCurrentVelocityRoot;
    private Dictionary<HandFinger, float> desiredWeights = new Dictionary<HandFinger, float>();
    private Dictionary<HandFinger, float> smoothDampCurrentVelocities = new Dictionary<HandFinger, float>();

    [SerializeField] [Range(0, 1)] private float surfaceMaxUpWeight = 0.5f;
    [SerializeField] [Range(0, 1)] private float surfaceMaxDownWeight = 0.5f;
    [SerializeField] [Range(0, 1)] private float globalRotationWeight = 1f;

    [SerializeField] [Range(0, 1)] private float fingerWeightSmoothTime = .5f;
    [SerializeField] [Range(0, 0.5f)] private float fingerTargetPositionSmoothTime = .05f;
    [SerializeField] private float gravitationCastDownDistance = .01f, gravitationCastUpDistance = .01f;

    [SerializeField] private float surfacePositionOffset = -.01f;

    [SerializeField] private LayerMask surfaceDetectionLayers;
    [SerializeField] private LayerMask snapPointDetectionLayers;
    */

    private void Awake()
    {
        _name = "StaticHand";

        _deviceSlots =
        new Dictionary<string, DeviceSlot>() {
                {
                    "Controller",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.devicePosition,
                            (DeviceFeatureUsage)CommonDeviceFeatures.deviceRotation,
                            (DeviceFeatureUsage)CommonDeviceFeatures.gripButton
                        }
                    }
                }
        };

        allSkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        pivot = GetComponentInChildren<Pivot>();

        //make invisible
        allSkinnedMeshRenderers.ToList().ForEach(smr => { smr.enabled = false; });

        allDropZones = FindObjectsOfType<StaticHandGrabbableDropZone>().ToList();
    }

    private void Update()
    {
        if (Devices.ContainsKey("Controller"))
        {
            //Set handedness.
            handedness = Devices["Controller"].Characteristics.HasFlag(DeviceCharacteristics.Left) ? Handedness.Left : Handedness.Right;
            if (handedness == Handedness.None)
            {
                return;
            }
            else if (handedness == Handedness.Left)
            {
                meshRenderer.enabled = !setInvisible;
                pivot.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (handedness == Handedness.Right)
            {
                meshRenderer.enabled = !setInvisible;
                pivot.transform.localScale = new Vector3(1, 1, -1);
            }

            //Get input.
            Devices["Controller"].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 outPos);
            Devices["Controller"].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion outRot);
            Devices["Controller"].TryGetFeatureValue(CommonDeviceFeatures.gripButton, out bool outGrip);
            Devices["Controller"].TryGetFeatureValue(CommonDeviceFeatures.primary2DAxisTouch, out bool outThumb);

            transform.position = outPos;
            transform.rotation = outRot;

            gripState += Time.deltaTime * (1 / timeToFullyGrip) * (outGrip || alwaysGrip ? 1 : -1);
            gripState = Mathf.Clamp01(gripState);

            thumbState += Time.deltaTime * (1 / timeToFullyGrip) * (outThumb ? 1 : -1);
            thumbState = Mathf.Clamp01(thumbState);

            //Grab things.
            if (grabTarget == null && gripState == 1 && DropZoneValid)
            {
                foreach (Collider collider in Physics.OverlapSphere(transform.position, grabRadius))
                {
                    StaticHandGrabbable grabbable = collider.GetComponent<StaticHandGrabbable>();
                    if (grabbable)
                    {
                        if(!grabbable.isBeingGrabbed)
                        {
                            Grab(grabbable);
                            break;
                        }
                    }
                }
            }
            //Drop things.
            else if(grabTarget != null & gripState != 1 && DropZoneValid)
            {
                if(!grabTarget.sticksToHandAfterBeingGrabbed)
                    Drop();
            }



            //Set the target positions for the IK rig based on whether or not the hand is gripping or not, and whether or not it's grabbing something.
            if(grabTarget == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Transform target = ikTargetPositions.GetChild(i);
                    Transform loose = looseTargetPositions.GetChild(i);
                    Transform gripped = grippedTargetPositions.GetChild(i);

                    Vector3 targetPos = Vector3.Lerp(loose.position, gripped.position, i != 4 ? gripState : thumbState);
                    Quaternion targetRot = Quaternion.Lerp(loose.rotation, gripped.rotation, i != 4 ? gripState : thumbState);

                    target.SetPositionAndRotation(targetPos, targetRot);
                }
            }
            else
            {
                grabTarget.transform.SetPositionAndRotation(transform.position, transform.rotation);

                for (int i = 0; i < 5; i++)
                {
                    Transform target = ikTargetPositions.GetChild(i);
                    Transform grab = handedness == Handedness.Left ? grabTarget.leftHandIkTargets.GetChild(i) : grabTarget.rightHandIkTargets.GetChild(i);

                    target.SetPositionAndRotation(grab.position, grab.rotation);
                }
            }
        }
        else
        {
            meshRenderer.enabled = false;
        }

        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    public enum Handedness
    {
        None,
        Left,
        Right
    }

    private void Grab(StaticHandGrabbable grabTarget)
    {
        this.grabTarget = grabTarget;
        grabTarget.BeGrabbed(this);
    }

    private void Drop()
    {
        grabTarget.BeDropped(lastPos, transform.position);
        grabTarget = null;
    }

    /*
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

                foreach (HandFinger hf in FingerTransforms.Keys)
                {
                    var target = FingerTransforms[hf];
                    var otherTarget = other.FingerTransforms[hf];
                    if (otherTarget.target != null)
                    {
                        target.SetPositionAndRotation(otherTarget, immediate);
                    }
                }
            }
        }

        public FingerTarget GetTargetForFinger(HandFinger hf)
        {
            switch (hf)
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

            foreach (FingerTarget target in FingerTransforms.Values)
            {
                target.UpdateTargetPositionAndRotation(fingerTargetPositionSmoothTime);
            }
        }
    }
    */

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
            if (immediate)
            {
                target.position = desiredPosition;
                target.rotation = desiredRotation;
            }
        }

        public void UpdateTargetPositionAndRotation(float fingerTargetPositionSmoothTime)
        {
            target.position = Vector3.SmoothDamp(target.position, desiredPosition, ref currentVelocity, fingerTargetPositionSmoothTime);
            target.rotation = SmoothDamp(target.rotation, desiredRotation, ref derivRot, .1f);
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
