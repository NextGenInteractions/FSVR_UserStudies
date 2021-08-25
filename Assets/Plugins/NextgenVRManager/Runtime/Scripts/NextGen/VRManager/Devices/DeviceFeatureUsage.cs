using System;
using System.Collections.Generic;

namespace NextGen.VrManager.Devices
{
    /// <summary>
    /// A device feature usage defines a feature of a device, and what type of data that feature gives.
    /// Check CommonDeviceFeatures for some common examples
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct DeviceFeatureUsage<T> : IEquatable<DeviceFeatureUsage<T>>
    {
        public DeviceFeatureUsage(string usageName)
        {
            name = usageName;
        }

        public string name { readonly get; set; }

        public override bool Equals(object obj)
        {
            return obj is DeviceFeatureUsage<T> usage &&
                   name == usage.name;
        }

        public bool Equals(DeviceFeatureUsage<T> other)
        {
            return other.name == this.name;
        }

        public override int GetHashCode()
        {
            return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
        }

        public static bool operator ==(DeviceFeatureUsage<T> a, DeviceFeatureUsage<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DeviceFeatureUsage<T> a, DeviceFeatureUsage<T> b)
        {
            return !a.Equals(b);
        }

        public static explicit operator DeviceFeatureUsage(DeviceFeatureUsage<T> self)
        {
            return new DeviceFeatureUsage { name = self.name, type = typeof(T) };
        }
    }

    public struct DeviceFeatureUsage : IEquatable<DeviceFeatureUsage>
    {
        public string name { get; internal set; }
        public Type type { get; internal set; }

        public DeviceFeatureUsage<T> As<T>()
        {
            return new DeviceFeatureUsage<T>() { name = name };
        }

        public override bool Equals(object obj)
        {
            return obj is DeviceFeatureUsage usage &&
                   name == usage.name && type == usage.type;
        }

        public bool Equals(DeviceFeatureUsage other)
        {
            return other.name == this.name
                && other.type == this.type;
        }

        public override int GetHashCode()
        {
            int hashCode = 1725085987;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(type);
            return hashCode;
        }

        public static bool operator ==(DeviceFeatureUsage a, DeviceFeatureUsage b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(DeviceFeatureUsage a, DeviceFeatureUsage b)
        {
            return !a.Equals(b);
        }
    }
}