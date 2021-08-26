using NextGen.VrManager.Devices;
using NextGen.VrManager.Devices.Hands;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NextGen.VrManager.DebugTools
{
    public class HandDeviceVisualization : SimpleDeviceVisualization
    {
        [SerializeField]
        bool debug;

        [SerializeField]
        HandModelConfig handModel;

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            if (device.TryGetFeatureValue(CommonDeviceFeatures.handData, out Hand hand))
            {
                bool isLeft = device.Characteristics.HasFlag(DeviceCharacteristics.Left);

                handModel.transformToFlip.localScale = isLeft ? handModel.leftScale : handModel.rightScale;
                handModel.handRenderer.enabled = true;

                if (hand.TryGetRootBone(out Bone bone))
                {
                    if(bone.TryGetPosition(out Vector3 position) && bone.TryGetRotation(out Quaternion rotation))
                    {
                        transform.SetPositionAndRotation(position, rotation);
                        if(debug)
                        {
                            Debug.DrawLine(position - Vector3.up * .01f, position + Vector3.up * .02f);
                            Debug.DrawLine(position - Vector3.right * .01f, position + Vector3.right * .02f);
                        }
                    }
                }
                foreach (HandFinger handFinger in Enum.GetValues(typeof(HandFinger)))
                {
                    var bones = new List<Bone>();
                    if (hand.TryGetFingerBones(handFinger, bones))
                    {
                        var transforms = handModel.GetTransforms(handFinger);
                        for (int i = 0; i < bones.Count; i++)
                        {
                            var currBone = bones[i];
                            if(currBone.TryGetPosition(out Vector3 position) && bone.TryGetRotation(out Quaternion rotation))
                            {
                                if (i < transforms.Length)
                                    transforms[i].SetPositionAndRotation(position, rotation);

                                if(debug)
                                {
                                    Debug.DrawLine(position - Vector3.up * .01f, position + Vector3.up * .02f);
                                    Debug.DrawLine(position - Vector3.right * .01f, position + Vector3.right * .02f);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                handModel.handRenderer.enabled = false;
            }
        }
    }
}