using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using TMPro;
using NextGen.VrManager.Devices.Serials;

namespace NextGen.VrManager.Ui
{
    public class TouchpadFeatureUiWidget : InspectorUiWidget
    {
        private TextMeshProUGUI text;

        public RectTransform visualizerCursor;

        public void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Update()
        {
            nextGenInspectorUi.Device.TryGetFeatureValue(SerialDeviceFeatures.touchpad, out Vector2 getTouchpad);
            nextGenInspectorUi.Device.TryGetFeatureValue(SerialDeviceFeatures.touchpadTouch, out bool getTouchpadTouch);

            text.text = "Touchpad: " + getTouchpad;

            visualizerCursor.gameObject.SetActive(getTouchpadTouch);
            if (getTouchpadTouch)
                visualizerCursor.anchoredPosition = new Vector2((getTouchpad.x * 10) - 10, (getTouchpad.y * 10) - 10);
        }
    }
}
