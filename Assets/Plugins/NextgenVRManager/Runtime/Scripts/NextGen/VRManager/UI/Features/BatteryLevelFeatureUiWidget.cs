using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using TMPro;

namespace NextGen.VrManager.Ui
{
    public class BatteryLevelFeatureUiWidget : InspectorUiWidget
    {
        private TextMeshProUGUI text;

        public void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Update()
        {
            nextGenInspectorUi.Device.TryGetFeatureValue(CommonDeviceFeatures.batteryLevel, out float getBatteryLevel);
            text.text = "Battery Level: " + string.Format("{0:P0}", getBatteryLevel);
        }
    }
}
