using System;
using System.Collections.Generic;

namespace NextGen.VrManager.Devices.Hands
{
    public struct Hand : IEquatable<Hand>
    {
        private Func<HandFinger, List<Bone>, bool> _getFingerBonesFunc;
        private Bone.TryGetOutDelegate<Bone> _getRootBoneFunc;

        public Hand(Func<HandFinger, List<Bone>, bool> getFingerBonesFunc, Bone.TryGetOutDelegate<Bone> getRootBoneFunc)
        {
            _getFingerBonesFunc = getFingerBonesFunc;
            _getRootBoneFunc = getRootBoneFunc;
        }

        public bool TryGetFingerBones(HandFinger finger, List<Bone> bonesOut)
        {
            return _getFingerBonesFunc(finger, bonesOut);
        }
        public bool TryGetRootBone(out Bone boneOut)
        {
            return _getRootBoneFunc(out boneOut);
        }

        #region Equatable
        public bool Equals(Hand other)
        {
            throw new NotImplementedException();
        }

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

        public static bool operator ==(Hand a, Hand b)
        {
            return a != null && a.Equals(b);
        }

        public static bool operator !=(Hand a, Hand b)
        {
            return !(a == b);
        }
        #endregion
    }
}