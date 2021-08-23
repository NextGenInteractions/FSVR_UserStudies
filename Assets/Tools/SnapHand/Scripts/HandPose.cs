public struct HandPose
{
    public Bone[] IndexBones, MiddleBones, RingBones, PinkyBones, ThumbBones;

    public HandPose(Bone[] indexBones, Bone[] middleBones, Bone[] ringBones, Bone[] pinkyBones, Bone[] thumbBones)
    {
        IndexBones = indexBones;
        MiddleBones = middleBones;
        RingBones = ringBones;
        PinkyBones = pinkyBones;
        ThumbBones = thumbBones;
    }

    public class Bone
    {
        public float localPositionX, localPositionY, localPositionZ;
        public float localRotationX, localRotationY, localRotationZ, localRotationW;
    }
}

