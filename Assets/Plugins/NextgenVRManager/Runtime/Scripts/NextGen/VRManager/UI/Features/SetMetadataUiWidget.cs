using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using TMPro;
using UnityEngine.UI;

namespace NextGen.VrManager.Ui
{
    public class SetMetadataUiWidget : InspectorUiWidget
    {
        private TextMeshProUGUI text;

        public void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            GetComponentInChildren<Button>().onClick.AddListener(SetMetadata);
        }

        public void SetMetadata()
        {
            DialogManager.SetMetadataDialog(nextGenInspectorUi.Device);
        }
    }
}
