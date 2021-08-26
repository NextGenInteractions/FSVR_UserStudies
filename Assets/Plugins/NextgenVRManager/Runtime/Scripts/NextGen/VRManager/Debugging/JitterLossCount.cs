using NextGen.VrManager.Devices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextGen.VrManager.DebugTools
{
    public class JitterLossCount : MonoBehaviour
    {
        public Device device;

        public int jitters = 0;
        public int losses = 0;

        public float timeSinceLastTrack = 0;
        private bool hasJittered = false;
        private bool hasLost = false;

        public Vector3 position;
        public Quaternion rotation;

        public void Init(Device _device)
        {
            device = _device;
        }

        public void Update()
        {
            device.TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out position);
            device.TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out rotation);

            if (position == Vector3.zero)
            {
                timeSinceLastTrack += Time.deltaTime;

                if(timeSinceLastTrack > JitterLossCounter.jitterThreshold && !hasJittered)
                {
                    hasJittered = true;
                    jitters++;
                }

                if(timeSinceLastTrack > JitterLossCounter.lossThreshold && !hasLost)
                {
                    hasLost = true;
                    jitters--;
                    losses++;
                }

            }
            else
            {
                timeSinceLastTrack = 0;
                hasJittered = false;
                hasLost = false;
            }
        }

        public void Reset()
        {
            jitters = 0;
            losses = 0;
            timeSinceLastTrack = 0;
        }
    }
}
