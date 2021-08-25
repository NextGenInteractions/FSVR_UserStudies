using System.Linq;
using UnityEngine;

namespace NextGen.VrManager.Devices.LeapMotion
{
    public class LeapHandDeviceManager : MonoBehaviour
    {
        LeapHand[] hands;

        private void Awake()
        {
            hands = FindObjectsOfType<LeapHand>();
        }

        // Update is called once per frame
        void Update()
        {
            foreach(LeapHand hand in hands)
            {
                var name = $"Leap Motion {(hand.Handedness == Leap.Unity.Chirality.Left ? "Left" : "Right")} Hand";

                LeapHandDevice device = (LeapHandDevice)DeviceManager.ActiveDevices.FirstOrDefault((device) => { return device.Uid == name; });

                if (device == null && hand.GetLeapHand() != null)
                {
                    LeapHandDevice handDevice = new LeapHandDevice(hand);

                    DeviceManager.AddDevice(handDevice);
                }
            }
        }
    }
}
