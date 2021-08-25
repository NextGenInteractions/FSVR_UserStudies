using Leap;
using Leap.Unity;
using UnityEngine;

namespace NextGen.VrManager.Devices.LeapMotion
{
    public class LeapHand : HandModelBase
    {
        [SerializeField]
        private Chirality _handedness;

        private Hand _hand;

        public override Chirality Handedness { get => _handedness; set => _handedness = value; }

        public override ModelType HandModelType => ModelType.Graphics;

        public override Hand GetLeapHand()
        {
            return IsTracked ? _hand : null;
        }

        public override void SetLeapHand(Hand hand)
        {
            _hand = hand;
        }

        public override void UpdateHand()
        {
        }
    }
}
