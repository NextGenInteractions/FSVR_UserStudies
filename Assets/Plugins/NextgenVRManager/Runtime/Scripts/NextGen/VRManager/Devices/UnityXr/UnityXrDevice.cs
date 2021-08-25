using NextGen.VrManager.Devices.SteamVr;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace NextGen.VrManager.Devices.UnityXr
{
    /// <summary>
    /// Provides a map from a InputDevice coming in through UnityXR to our Device layer
    /// </summary>
    public class UnityXrDevice : Device
    {
        public InputDeviceWrapper InputDevice { get; private set; }

        private List<InputFeatureUsage> inputFeatures = new List<InputFeatureUsage>();

        public void SetData(InputDeviceWrapper _device)
        {
            InputDevice = _device;

            Uid = InputDevice.serialNumber;
            Name = InputDevice.name;

            MapCharacteristics(InputDevice.characteristics);

            MapFeatures(InputDevice);
        }

        public void RefreshFeatures()
        {
            if(InputDevice.RefreshXRNode())
            {
                MapFeatures(InputDevice);
            }
        }

        private void MapCharacteristics(InputDeviceCharacteristics characteristics)
        {
            Characteristics = DeviceCharacteristics.None;

            if(characteristics.HasFlag(InputDeviceCharacteristics.Camera))
                Characteristics |= DeviceCharacteristics.Camera;
            if (characteristics.HasFlag(InputDeviceCharacteristics.Controller))
                Characteristics |= DeviceCharacteristics.Controller;
            if (characteristics.HasFlag(InputDeviceCharacteristics.EyeTracking))
                Characteristics |= DeviceCharacteristics.EyeTracking;
            if (characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
                Characteristics |= DeviceCharacteristics.HandTracking;
            if (characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
                Characteristics |= DeviceCharacteristics.HeadMounted;
            if (characteristics.HasFlag(InputDeviceCharacteristics.HeldInHand))
                Characteristics |= DeviceCharacteristics.HeldInHand;
            if (characteristics.HasFlag(InputDeviceCharacteristics.Left))
                Characteristics |= DeviceCharacteristics.Left;
            if (characteristics.HasFlag(InputDeviceCharacteristics.Right))
                Characteristics |= DeviceCharacteristics.Right;
            if (characteristics.HasFlag(InputDeviceCharacteristics.Simulated6DOF))
                Characteristics |= DeviceCharacteristics.Simulated6DOF;
            if (characteristics.HasFlag(InputDeviceCharacteristics.TrackedDevice))
                Characteristics |= DeviceCharacteristics.TrackedDevice;
            if (characteristics.HasFlag(InputDeviceCharacteristics.TrackingReference))
                Characteristics |= DeviceCharacteristics.TrackingReference;
        }

        private void MapFeatures(InputDeviceWrapper inputDevice)
        {
            if (InputDevice.TryGetFeatureUsages(inputFeatures))
            {
                foreach (InputFeatureUsage f in inputFeatures)
                {
                    TryGetOutDelegate<object> getFeatureValueFunction = (out object obj) => { obj = default; return false; };
                    Type returnType = f.type;

                    if (f.type == typeof(bool))
                        getFeatureValueFunction = (out object obj) => {
                            bool wasSuccessful = inputDevice.TryGetFeatureValue(f.As<bool>(), out bool value);
                            obj = value;
                            return wasSuccessful;
                        };
                    else if (f.type == typeof(uint))
                        getFeatureValueFunction = (out object obj) => {
                            bool wasSuccessful = inputDevice.TryGetFeatureValue(f.As<uint>(), out uint value);
                            obj = value;
                            return wasSuccessful;
                        };
                    else if (f.type == typeof(float))
                        getFeatureValueFunction = (out object obj) => {
                            bool wasSuccessful = inputDevice.TryGetFeatureValue(f.As<float>(), out float value);
                            obj = value;
                            return wasSuccessful;
                        };
                    else if (f.type == typeof(Vector2))
                        getFeatureValueFunction = (out object obj) => {
                            bool wasSuccessful = inputDevice.TryGetFeatureValue(f.As<Vector2>(), out Vector2 value);
                            obj = value;
                            return wasSuccessful;
                        };
                    else if (f.type == typeof(Vector3))
                        getFeatureValueFunction = (out object obj) => {
                            bool wasSuccessful = inputDevice.TryGetFeatureValue(f.As<Vector3>(), out Vector3 value);
                            obj = value;
                            return wasSuccessful;
                        };
                    else if (f.type == typeof(Quaternion))
                        getFeatureValueFunction = (out object obj) => {
                            bool wasSuccessful = inputDevice.TryGetFeatureValue(f.As<Quaternion>(), out Quaternion value);
                            obj = value;
                            return wasSuccessful;
                        };
                    else if (f.type == typeof(byte[]))
                    {
                        //unsupported for now, this one is more complex and may be feature-dependent
                    }
                    else if (f.type == typeof(Hand))
                    {
                        //map UnityXR Hand to NextGen Hand
                        getFeatureValueFunction = (out object obj) => {
                            bool wasSuccessful = inputDevice.TryGetFeatureValue(f.As<Hand>(), out Hand value);
                            obj = ConvertToNextGenHand(value);
                            return wasSuccessful;
                        };
                        returnType = typeof(Hands.Hand);
                    }

                    featureValues[new DeviceFeatureUsage() { name = f.name, type = returnType }] = getFeatureValueFunction;
                }
            }
        }

        private Hands.Hand ConvertToNextGenHand(Hand h)
        {
            return new Hands.Hand(
                (Hands.HandFinger hf, List<Hands.Bone> bones) =>
                {
                    List<Bone> xrBones = new List<Bone>();

                    bool gotBones = h.TryGetFingerBones(ConvertToXrHandFinger(hf), xrBones);

                    if (gotBones)
                    {
                        bones.AddRange(ConvertToNextGenBones(xrBones));
                    }

                    return gotBones;
                },
                (out Hands.Bone bone) =>
                {
                    bool hasBone = h.TryGetRootBone(out Bone xrRootBone);

                    bone = hasBone ? ConvertToNextGenBone(xrRootBone) : default;

                    return hasBone;
                });
        }

        private HandFinger ConvertToXrHandFinger(Hands.HandFinger handFinger)
        {
            switch (handFinger)
            {
                case Hands.HandFinger.Index:
                    return HandFinger.Index;
                case Hands.HandFinger.Middle:
                    return HandFinger.Middle;
                case Hands.HandFinger.Ring:
                    return HandFinger.Ring;
                case Hands.HandFinger.Pinky:
                    return HandFinger.Pinky;
                case Hands.HandFinger.Thumb:
                    return HandFinger.Thumb;
                default:
                    return HandFinger.Thumb;
            }
        }

        private List<Hands.Bone> ConvertToNextGenBones(List<Bone> xrBones)
        {
            return xrBones.Select((xrBone) => { return ConvertToNextGenBone(xrBone); }).ToList();
        }

        private Hands.Bone ConvertToNextGenBone(Bone xrBone)
        {
            return new Hands.Bone(
                xrBone.TryGetPosition,
                xrBone.TryGetRotation);
        }
    }
}