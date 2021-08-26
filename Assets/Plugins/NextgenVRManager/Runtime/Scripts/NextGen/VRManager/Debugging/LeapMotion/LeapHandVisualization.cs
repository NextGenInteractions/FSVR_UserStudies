using Leap;
using Leap.Unity;
using NextGen.VrManager.Devices.LeapMotion;

namespace NextGen.VrManager.DebugTools.LeapMotion
{
    public class LeapHandVisualization : SimpleDeviceVisualization
    {
        private HandModelBase handModel { get; set; }

        private LeapHandDevice leapHandDevice { get; set; }

        public override void Init(Devices.Device h)
        {
            handModel = GetComponentInChildren<HandModelBase>();

            base.Init(h);

            leapHandDevice = (LeapHandDevice)h;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            if (leapHandDevice.TryGetFeatureValue(LeapDeviceFeatures.handData, out Hand hand))
            {
                handModel.SetLeapHand(hand);

                if(hand != null)
                    handModel.UpdateHand();
            }
        }
    }
}