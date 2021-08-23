using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.Tools.Ui
{
    public class BackscatterUiWidget : ToolInspectorUiWidget
    {
        public Backscatter backscatter;

        public DragAndDropSlot dragAndDropSlot;
        private Image dragAndDropIcon;
        public TextMeshProUGUI dragAndDropLabel;

        public override Type ToolType => typeof(Backscatter);

        private void Start()
        {
            if (nextGenInspectorUi)
                backscatter = (Backscatter)nextGenInspectorUi.Tool;

            dragAndDropIcon = dragAndDropSlot.transform.GetChild(0).GetComponent<Image>();

            dragAndDropSlot.slotRequirements = backscatter.DeviceSlots["Backscatter"];
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            dragAndDropSlot.deviceSet += SetDevice;
        }

        // Update is called once per frame
        void Update()
        {
            dragAndDropIcon.sprite = backscatter.Devices.ContainsKey("Backscatter") ? DeviceManagerUi.GetIconSpriteForDevice(backscatter.Devices[("Backscatter")]) : DeviceManagerUi.Instance.nullIconSprite;
            dragAndDropIcon.color = backscatter.Devices.ContainsKey("Backscatter") ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            dragAndDropLabel.text = backscatter.GetLabelForSlot("Backscatter");
            dragAndDropLabel.color = backscatter.Devices.ContainsKey("Backscatter") ? Color.white : new Color(1, 1, 1, 0.5f);
        }

        public void SetDevice(Device d)
        {
            backscatter.SetDevice("Backscatter", d);
        }
    }
}
