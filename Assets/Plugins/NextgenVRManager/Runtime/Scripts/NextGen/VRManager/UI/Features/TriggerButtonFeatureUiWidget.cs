using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using TMPro;

namespace NextGen.VrManager.Ui
{
    public class TriggerButtonFeatureUiWidget : InspectorUiWidget
    {
        private TextMeshProUGUI text;

        public void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Update()
        {
            nextGenInspectorUi.Device.TryGetFeatureValue(CommonDeviceFeatures.triggerButton, out bool getTriggerButton);
            text.text = "Trigger Button: " + getTriggerButton;
        }
    }
}
