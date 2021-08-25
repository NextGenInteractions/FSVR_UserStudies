////MODIFIED BY TONY! (and NextGen Interactions)
////
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

#if UNITY_STANDALONE
using Valve.VR;
#endif

namespace NextGen.VrManager.Devices.SteamVr
{
    /// <summary>
    /// Wraps an input device for the XR Interaction toolkit to add support for SteamVR Input System
    /// </summary>
    public class InputDeviceWrapper
    {
        /// <summary>
        /// The wrapped Input Device. We'll take positions and rotations from it in any case.
        /// It will also provide inputs with non-SteamVR headsets
        /// </summary>
        private InputDevice m_inputDevice;

        /// <summary>
        /// Node we must provide input from
        /// </summary>
        private SteamVR_Input_Sources m_inputSource;

        /// <summary>
        /// True if there is steamvr activated, false otherwise
        /// </summary>
        private bool m_isSteamVR;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceNode">Device from which take the input</param>
        internal InputDeviceWrapper(InputDevice inputDevice)
        {
            var isSteamVR = inputDevice.subsystem != null && inputDevice.subsystem.SubsystemDescriptor.id == "OpenVR Input";

            m_inputDevice = inputDevice;
            m_isSteamVR = isSteamVR;
            m_inputSource = SteamVR_Input_Sources.Any;
            
            RefreshXRNode();
        }
        /// <summary>
        /// Refreshes XR node, returns true if XRNode changed
        /// </summary>
        /// <returns></returns>
        public bool RefreshXRNode()
        {
            //Don't refresh if a XR node has already been set, or the device is not a steamVR device
            if(m_isSteamVR && m_inputSource == SteamVR_Input_Sources.Any)
            {
                if (m_inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
                {
                    m_inputSource = SteamVR_Input_Sources.Head;
                }

                foreach (SteamVR_Input_Sources source in SteamVR_Input_Source.GetAllSources())
                {
                    if(source != SteamVR_Input_Sources.Any)
                    {
                        var pose = SteamVR_Actions.default_Pose[source];

                        if (pose.active)
                        {
                            var index = pose.trackedDeviceIndex;

                            ETrackedPropertyError error = new ETrackedPropertyError();
                            StringBuilder sb = new StringBuilder();
                            OpenVR.System.GetStringTrackedDeviceProperty(index, ETrackedDeviceProperty.Prop_SerialNumber_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
                            var SerialNumber = sb.ToString();

                            if (SerialNumber == m_inputDevice.serialNumber)
                            {
                                m_inputSource = source;
                                Debug.Log($"{m_inputDevice.name} mapped to {m_inputSource}");
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///   <para>Read Only. True if the device is currently a valid input device; otherwise false.</para>
        /// </summary>
        public bool isValid
        {
            get
            {
                return m_inputDevice.isValid;
            }
        }

        /// <summary>
        ///   <para>Read Only. The name of the device in the XR system. This is a platform provided unique identifier for the device.</para>
        /// </summary>
        public string name
        {
            get
            {
                return m_inputDevice.name;
            }
        }

        /// <summary>
        ///   <para>Read Only. The InputDeviceRole of the device in the XR system. This is a platform provided description of how the device is used.</para>
        /// </summary>
        [Obsolete("This API has been marked as deprecated and will be removed in future versions. Please use InputDevice.characteristics instead.")]
        public InputDeviceRole role
        {
            get
            {
                return m_inputDevice.role;
            }
        }

        /// <summary>
        ///   <para>The manufacturer of the connected Input Device.</para>
        /// </summary>
        public string manufacturer
        {
            get
            {
                return m_inputDevice.manufacturer;
            }
        }

        /// <summary>
        ///   <para>The serial number of the connected Input Device.  Blank if no serial number is available.</para>
        /// </summary>
        public string serialNumber
        {
            get
            {
                return m_inputDevice.serialNumber;
            }
        }

        /// <summary>
        ///   <para>Read Only. A bitmask of enumerated flags describing the characteristics of this InputDevice.</para>
        /// </summary>
        public InputDeviceCharacteristics characteristics
        {
            get
            {
                return m_inputDevice.characteristics;
            }
        }

        /// <summary>
        ///   <para>Sends a haptic impulse to a device.</para>
        /// </summary>
        /// <param name="channel">The channel to receive the impulse.</param>
        /// <param name="amplitude">The normalized (0.0 to 1.0) amplitude value of the haptic impulse to play on the device.</param>
        /// <param name="duration">The duration in seconds that the haptic impulse will play. Only supported on Oculus.</param>
        /// <returns>
        ///   <para>Returns true if successful. Returns false otherwise.</para>
        /// </returns>
        public bool SendHapticImpulse(uint channel, float amplitude, float duration = 1f)
        {
            return m_inputDevice.SendHapticImpulse(channel, amplitude, duration);
        }

        /// <summary>
        ///   <para>Sends a raw buffer of haptic data to the device.</para>
        /// </summary>
        /// <param name="channel">The channel to receive the data.</param>
        /// <param name="buffer">A raw byte buffer that contains the haptic data to send to the device.</param>
        /// <returns>
        ///   <para>Returns true if successful. Returns false otherwise.</para>
        /// </returns>
        public bool SendHapticBuffer(uint channel, byte[] buffer)
        {
            return m_inputDevice.SendHapticBuffer(channel, buffer);
        }

        public bool TryGetHapticCapabilities(out HapticCapabilities capabilities)
        {
            return m_inputDevice.TryGetHapticCapabilities(out capabilities);
        }

        /// <summary>
        ///   <para>Stop all haptic playback for a device.</para>
        /// </summary>
        public void StopHaptics()
        {
            m_inputDevice.StopHaptics();
        }

        public bool TryGetFeatureUsages(List<InputFeatureUsage> featureUsages)
        {
            var retVal = m_inputDevice.TryGetFeatureUsages(featureUsages);

#if UNITY_STANDALONE
            if (hasBatteryData())
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.batteryLevel);
            }

            if (m_isSteamVR && m_inputSource.CanHaveInput())
            {
                featureUsages.Add((InputFeatureUsage)CommonUsages.triggerButton);
                featureUsages.Add((InputFeatureUsage)CommonUsages.trigger);

                featureUsages.Add((InputFeatureUsage)CommonUsages.gripButton);

                featureUsages.Add((InputFeatureUsage)CommonUsages.primary2DAxisTouch);
                featureUsages.Add((InputFeatureUsage)CommonUsages.primary2DAxis);
                featureUsages.Add((InputFeatureUsage)CommonUsages.primary2DAxisClick);

                featureUsages.Add((InputFeatureUsage)CommonUsages.menuButton);
                
                if (name.ToLower().Contains("knuckles"))
                {
                    featureUsages.Add((InputFeatureUsage)CommonUsages.primaryButton);
                    featureUsages.Add((InputFeatureUsage)CommonUsages.secondaryButton);
                    featureUsages.Add((InputFeatureUsage)CommonUsages.secondary2DAxis);
                    featureUsages.Add((InputFeatureUsage)CommonUsages.secondary2DAxisTouch);
                }
            }
#endif
            return retVal;
        }

        public bool TryGetFeatureValue(InputFeatureUsage<bool> usage, out bool value)
        {
#if UNITY_STANDALONE
            if (m_isSteamVR && m_inputSource.CanHaveInput())
            {
                if (usage == CommonUsages.triggerButton)
                {
                    value = SteamVR_Actions.ngvrmanager.TriggerClick[m_inputSource].state;

                    return true;
                }
                else if (usage == CommonUsages.gripButton)
                {
                    value = SteamVR_Actions.ngvrmanager.Grip[m_inputSource].state;

                    return true;
                }
                else if (usage == CommonUsages.primary2DAxisTouch)
                {
                    value = SteamVR_Actions.ngvrmanager.TouchpadTouching[m_inputSource].state;

                    return true;
                }
                else if (usage == CommonUsages.primary2DAxisClick)
                {
                    value = SteamVR_Actions.ngvrmanager.TouchpadClick[m_inputSource].state;

                    return true;
                }
                else if (usage == CommonUsages.menuButton)
                {
                    value = SteamVR_Actions.ngvrmanager.MenuClick[m_inputSource].state;

                    return true;
                }
                else if (usage == CommonUsages.secondary2DAxisTouch)
                {
                    value = SteamVR_Actions.ngvrmanager.SecondaryTouchpadTouching[m_inputSource].state;

                    return true;
                }
                else if (usage == CommonUsages.primaryButton)
                {
                    value = SteamVR_Actions.ngvrmanager.PrimaryButton[m_inputSource].state;

                    return true;
                }
                else if (usage == CommonUsages.secondaryButton)
                {
                    value = SteamVR_Actions.ngvrmanager.SecondaryButton[m_inputSource].state;

                    return true;
                }
            }
#endif
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        private bool hasBatteryData()
        {
            return m_isSteamVR && m_inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.TrackedDevice)
                && !m_inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted)
                && !m_inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.TrackingReference);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<uint> usage, out uint value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<float> usage, out float value)
        {
            #if UNITY_STANDALONE
            if (hasBatteryData())
            {
                if (usage == CommonUsages.batteryLevel)
                {
                    if (SteamVrDeviceManager.Devices.ContainsKey(m_inputDevice.serialNumber))
                        value = SteamVrDeviceManager.Devices[m_inputDevice.serialNumber].batteryLevel;
                    else
                        value = -1;

                    return true;
                }
            }

            if (m_isSteamVR && m_inputSource.CanHaveInput())
            {
                if (usage == CommonUsages.trigger)
                {
                    value = SteamVR_Actions.ngvrmanager.TriggerPull[m_inputSource].axis;

                    return true;
                }
            }
#endif
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<Vector2> usage, out Vector2 value)
        {
#if UNITY_STANDALONE
            if (m_isSteamVR && m_inputSource.CanHaveInput())
            {
                if (usage == CommonUsages.primary2DAxis)
                {
                    value = SteamVR_Actions.ngvrmanager.TouchpadTouchpoint[m_inputSource].axis;

                    return true;
                }

                if (usage == CommonUsages.primary2DAxis)
                {
                    value = SteamVR_Actions.ngvrmanager.SecondaryTouchpadTouchpoint[m_inputSource].axis;

                    return true;
                }
            }
#endif
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<Vector3> usage, out Vector3 value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<Quaternion> usage, out Quaternion value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<Hand> usage, out Hand value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<UnityEngine.XR.Bone> usage, out UnityEngine.XR.Bone value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<Eyes> usage, out Eyes value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<byte[]> usage, byte[] value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, value);
        }

        public bool TryGetFeatureValue(
          InputFeatureUsage<InputTrackingState> usage,
          out InputTrackingState value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<bool> usage, DateTime time, out bool value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, time, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<uint> usage, DateTime time, out uint value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, time, out value);
        }

        public bool TryGetFeatureValue(InputFeatureUsage<float> usage, DateTime time, out float value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, time, out value);
        }

        public bool TryGetFeatureValue(
          InputFeatureUsage<Vector2> usage,
          DateTime time,
          out Vector2 value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, time, out value);
        }

        public bool TryGetFeatureValue(
          InputFeatureUsage<Vector3> usage,
          DateTime time,
          out Vector3 value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, time, out value);
        }

        public bool TryGetFeatureValue(
          InputFeatureUsage<Quaternion> usage,
          DateTime time,
          out Quaternion value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, time, out value);
        }

        public bool TryGetFeatureValue(
          InputFeatureUsage<InputTrackingState> usage,
          DateTime time,
          out InputTrackingState value)
        {
            return m_inputDevice.TryGetFeatureValue(usage, time, out value);
        }

    }
    #if UNITY_STANDALONE

    /// <summary>
    /// Helpers for use of XRNode input types with steam
    /// </summary>
    public static class InputXrNodeUtilities
    {
        /// <summary>
        /// True if the node represents a hand
        /// </summary>
        /// <param name="node"></param>
        public static bool CanHaveInput(this SteamVR_Input_Sources node)
        {
            return node != SteamVR_Input_Sources.Any && node != SteamVR_Input_Sources.Head;
        }
    }

#endif
}