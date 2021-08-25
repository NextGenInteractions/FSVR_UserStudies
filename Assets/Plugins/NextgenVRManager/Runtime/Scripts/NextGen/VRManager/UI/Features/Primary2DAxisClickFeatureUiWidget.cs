using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using TMPro;

namespace NextGen.VrManager.Ui
{
    public class Primary2DAxisClickFeatureUiWidget : InspectorUiWidget
    {
        private TextMeshProUGUI text;

        public void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Update()
        {
            nextGenInspectorUi.Device.TryGetFeatureValue(CommonDeviceFeatures.primary2DAxisClick, out bool getClick);
            text.text = "Primary 2D Axis Click: " + getClick;
        }
    }
}
