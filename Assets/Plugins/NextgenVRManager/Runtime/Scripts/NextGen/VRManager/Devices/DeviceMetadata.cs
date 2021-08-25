using System;
using System.Collections.Generic;

namespace NextGen.VrManager.Devices
{
    /// <summary>
    /// User-supplied data about this device
    /// </summary>
    [Serializable]
    public struct DeviceMetadata : IEquatable<DeviceMetadata>
    {
        public string Label { get; set; }
        public DeviceType Type { get; set; }
        public List<string> Tags { get; set; }

        public DeviceMetadata(string _label, int _type, List<string> _tags)
        {
            Label = _label;
            Type = (DeviceType)_type;
            Tags = _tags;
        }
        public DeviceMetadata(string _label, DeviceType _type, List<string> _tags)
        {
            Label = _label;
            Type = _type;
            Tags = _tags;
        }

        public bool Equals(DeviceMetadata other)
        {
            return Label == other.Label
                && Type == other.Type
                && Tags == other.Tags;
        }
    }

    [Serializable]
    public enum DeviceType
    {
        Lighthouse = 0,
        Wand = 1,
        Knuckles = 2,
        ViveHmd = 3,
        IndexHmd = 4,
        Puck = 5,
        Wristcuff = 6,
        Serial = 7,
        LeapHand = 8
    }
}