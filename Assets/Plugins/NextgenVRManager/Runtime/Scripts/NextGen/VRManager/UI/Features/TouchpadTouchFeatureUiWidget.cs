using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using TMPro;
using NextGen.VrManager.Devices.Serials;

namespace NextGen.VrManager.Ui
{
    public class TouchpadTouchFeatureUiWidget : InspectorUiWidget
    {
        private TextMeshProUGUI text;

        public void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Update()
        {
            nextGenInspectorUi.Device.TryGetFeatureValue(SerialDeviceFeatures.touchpadTouch, out bool getTouchpadTouch);
            text.text = "Touchpad Touch: " + getTouchpadTouch;
        }
    }
}
