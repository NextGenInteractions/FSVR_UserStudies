using System;
using UnityEngine;

namespace NextGen.VrManager.Devices.Hands
{
    public struct Bone : IEquatable<Bone>
    {
        private TryGetOutDelegate<Vector3> _getPositionFunc;
        private TryGetOutDelegate<Quaternion> _getRotationFunc;

        public delegate bool TryGetOutDelegate<T>(out T output);

        public bool TryGetPosition(out Vector3 position)
        {
            return _getPositionFunc(out position);
        }
        public bool TryGetRotation(out Quaternion rotation)
        {
            return _getRotationFunc(out rotation);
        }

        public Bone(TryGetOutDelegate<Vector3> getPositionFunc, TryGetOutDelegate<Quaternion> getRotationFunc)
        {
            _getPositionFunc = getPositionFunc;
            _getRotationFunc = getRotationFunc;
        }

        #region Equatable
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public bool Equals(Bone other)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Bone a, Bone b)
        {
            return a != null && a.Equals(b);
        }

        public static bool operator !=(Bone a, Bone b)
        {
            return !(a == b);
        }
        #endregion
    }
}