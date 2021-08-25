using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using TMPro;

namespace NextGen.VrManager.Ui
{
    public class Primary2DAxisTouchFeatureUiWidget : InspectorUiWidget
    {
        private TextMeshProUGUI text;

        public void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Update()
        {
            nextGenInspectorUi.Device.TryGetFeatureValue(CommonDeviceFeatures.primary2DAxisTouch, out bool getTouch);
            text.text = "Primary 2D Axis Touch: " + getTouch;
        }
    }
}
