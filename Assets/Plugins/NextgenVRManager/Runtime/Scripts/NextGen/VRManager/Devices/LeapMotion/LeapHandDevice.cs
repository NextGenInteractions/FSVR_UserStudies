using Leap;
using Leap.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NextGen.VrManager.Devices.LeapMotion
{
    public class LeapHandDevice : Device
    {
        Hand leapHand;
        Dictionary<Hands.HandFinger, Hands.Bone[]> fingerBones = new Dictionary<Hands.HandFinger, Hands.Bone[]>();
        Hands.Bone rootBone;

        Hands.Hand nextGenHandData;

        public LeapHandDevice(LeapHand _device)
        {
            Name = $"Leap Motion {(_device.GetLeapHand().IsLeft ? "Left" : "Right")} Hand";

            Characteristics = DeviceCharacteristics.None;

            Characteristics |= DeviceCharacteristics.HandTracking;
            Characteristics |= DeviceCharacteristics.TrackedDevice;
            Characteristics |= _device.GetLeapHand().IsLeft ? DeviceCharacteristics.Left : DeviceCharacteristics.Right;

            MapLeapHandToNextGenHand();

            //abstracted NextGen hand data
            featureValues[(DeviceFeatureUsage)CommonDeviceFeatures.handData] = (out object obj) => {
                leapHand = _device.IsTracked ? _device.GetLeapHand() : null;
                obj = _device.IsTracked ? nextGenHandData : default;
                return _device.IsTracked;
            };

            //raw leap hand data
            featureValues[(DeviceFeatureUsage)LeapDeviceFeatures.handData] = (out object obj) => {
                leapHand = _device.IsTracked ? _device.GetLeapHand() : null;
                obj = _device.IsTracked ? leapHand : default;
                return _device.IsTracked;
            };

            featureValues[(DeviceFeatureUsage)CommonDeviceFeatures.isTracked] = (out object obj) => { obj = _device.IsTracked;  return _device.IsTracked; }; //improve
            featureValues[(DeviceFeatureUsage)CommonDeviceFeatures.devicePosition] = (out object obj) => { obj = _device.IsTracked ? _device.GetLeapHand().PalmPosition.ToVector3() : default; return _device.IsTracked;  };
            featureValues[(DeviceFeatureUsage)CommonDeviceFeatures.deviceRotation] = (out object obj) => { obj = _device.IsTracked ? _device.GetLeapHand().Rotation.ToQuaternion() : default; return _device.IsTracked; };

            //for fun
            featureValues[(DeviceFeatureUsage)CommonDeviceFeatures.triggerButton] = (out object obj) => { obj = _device.IsTracked ? _device.GetLeapHand().IsPinching() : default; return _device.IsTracked; };
            featureValues[(DeviceFeatureUsage)CommonDeviceFeatures.trigger] = (out object obj) => { obj = _device.IsTracked ? _device.GetLeapHand().PinchStrength : default; return _device.IsTracked; };
        }

        public void MapLeapHandToNextGenHand()
        {
            rootBone = new Hands.Bone(
            (out Vector3 position) =>
            {
                position = leapHand.WristPosition.ToVector3();
                return true;
            },
            (out Quaternion rotation) =>
            {
                rotation = leapHand.Rotation.ToQuaternion();
                return true;
            });

            foreach (Hands.HandFinger handFinger in Enum.GetValues(typeof(Hands.HandFinger)))
            {
                fingerBones[handFinger] = ConvertToNextGenBones(handFinger);
            }

            nextGenHandData = new Hands.Hand(
                (Hands.HandFinger hf, List<Hands.Bone> bones) =>
                {
                    bones.AddRange(fingerBones[hf].ToList());
                    return true;
                },
                (out Hands.Bone bone) =>
                {
                    bone = rootBone;
                    return true;
                });
        }


        private Hands.Bone[] ConvertToNextGenBones(Hands.HandFinger hf)
        {
            var retVal = new Hands.Bone[4];
            for (int i = 0; i < retVal.Length; i++)
            {
                retVal[i] = ConvertToNextGenBone(hf, i);
            }
            return retVal;
        }

        private Hands.Bone ConvertToNextGenBone(Hands.HandFinger hf, int index)
        {
            return new Hands.Bone(
            (out Vector3 position) =>
            {
                position = GetFinger(leapHand, hf).bones[index].NextJoint.ToVector3();
                return true;
            },
            (out Quaternion rotation) =>
            {
                rotation = GetFinger(leapHand, hf).bones[index].Rotation.ToQuaternion();
                return true;
            });
        }

        private Finger GetFinger(Hand h, Hands.HandFinger hf)
        {
            switch (hf)
            {
                case Hands.HandFinger.Index:
                    return h.GetIndex();
                case Hands.HandFinger.Middle:
                    return h.GetMiddle();
                case Hands.HandFinger.Ring:
                    return h.GetRing();
                case Hands.HandFinger.Pinky:
                    return h.GetPinky();
                case Hands.HandFinger.Thumb:
                    return h.GetThumb();
                default:
                    return new Finger();
            }
        }
    }
}