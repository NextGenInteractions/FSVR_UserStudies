using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NextGen.VrManager.Devices
{
    /// <summary>
    /// The core of NextGenVRManager, Devices are generic objects that represent a hardware object, be it a vive wand, a headset, a lighthouse, leap motion hands, an arduino, and more!
    /// </summary>
    public abstract class Device
    {
        public delegate bool TryGetOutDelegate<T>(out T output);
        protected Dictionary<DeviceFeatureUsage, TryGetOutDelegate<object>> featureValues = new Dictionary<DeviceFeatureUsage, TryGetOutDelegate<object>>();

        //
        // Summary:
        //     Read Only. A bitmask of enumerated flags describing the characteristics of this
        //     InputDevice.
        public DeviceCharacteristics Characteristics { get; protected set; }

        // Summary:
        //     Read Only. The name of the device in the XR system. This is a platform provided
        //     unique identifier for the device.
        public string Name { get; protected set; }

        private string _uid;
        //
        // Summary:
        //     The unique identifier of the connected Input Device.
        public string Uid { get { return string.IsNullOrEmpty(_uid) ? Name : _uid; } protected set { _uid = value; } }

        /// <summary>
        /// User-supplied data about this device
        /// </summary>
        public DeviceMetadata? Metadata
        {
            get
            {
                return DeviceMetadataManager.GetMetadata(this);
            }
            set
            {
                if(value.HasValue)
                {
                    DeviceMetadataManager.SetMetadata(this, value.Value);
                }
            }
        }

        public string DisplayName
        {
            get
            {
                DeviceMetadata? metadata = Metadata;
                return metadata == null ? Name : metadata.Value.Label;
            }
        }

        public List<DeviceFeatureUsage> GetFeatures()
        {
            return featureValues.Keys.ToList();
        }

        public bool TryGetFeatureValue<T>(DeviceFeatureUsage<T> usage, out T outVal)
        {
            try
            {
                if (featureValues.ContainsKey((DeviceFeatureUsage)usage))
                {
                    if(featureValues[(DeviceFeatureUsage)usage](out object outObj))
                    {
                        outVal = (T)outObj;
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"Failed to get feature value for {usage}. Exception: {e.Message}");
            }
            

            outVal = default;
            return false;
        }
    }
}

