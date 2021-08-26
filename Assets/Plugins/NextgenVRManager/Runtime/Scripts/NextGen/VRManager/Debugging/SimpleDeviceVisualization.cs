using NextGen.VrManager.Devices;
using UnityEngine;

namespace NextGen.VrManager.DebugTools
{
    /// <summary>
    /// Part of the 
    /// </summary>
    public class SimpleDeviceVisualization : MonoBehaviour
    {
        public Device device { get; private set; }

        public virtual void Init(Device h)
        {
            device = h;
            name = $"{h.DisplayName}: {h.Name}";
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (device == null)
                return;

            device.TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos);
            device.TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot);

            transform.localPosition = getPos;
            transform.localRotation = getRot;
        }
    }
}