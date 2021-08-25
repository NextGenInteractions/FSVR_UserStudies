using NextGen.VrManager.Devices;
using TMPro;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public class Primary2DAxisFeatureUiWidget : InspectorUiWidget
    {
        private TextMeshProUGUI text;

        public void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Update()
        {
            nextGenInspectorUi.Device.TryGetFeatureValue(CommonDeviceFeatures.primary2DAxis, out Vector2 getAxis);
            text.text = "Primary 2D Axis: " + getAxis;
        }
    }
}
