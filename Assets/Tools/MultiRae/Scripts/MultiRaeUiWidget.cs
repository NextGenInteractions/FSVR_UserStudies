using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.Tools.Ui
{
    public class MultiRaeUiWidget : ToolInspectorUiWidget
    {
        public MultiRae multiRae;

        public DragAndDropSlot dragAndDropSlot;
        private Image dragAndDropIcon;
        public TextMeshProUGUI dragAndDropLabel;

        public override Type ToolType => typeof(MultiRae);

        private void Start()
        {
            if (nextGenInspectorUi)
                multiRae = (MultiRae)nextGenInspectorUi.Tool;

            dragAndDropIcon = dragAndDropSlot.transform.GetChild(0).GetComponent<Image>();

            dragAndDropSlot.slotRequirements = multiRae.DeviceSlots["Tracked Device with Menu, Trigger, Grip Buttons"];
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            dragAndDropSlot.deviceSet += SetDevice;
        }

        // Update is called once per frame
        void Update()
        {
            dragAndDropIcon.sprite = multiRae.Devices.ContainsKey("Tracked Device with Menu, Trigger, Grip Buttons") ? DeviceManagerUi.GetIconSpriteForDevice(multiRae.Devices[("Tracked Device with Menu, Trigger, Grip Buttons")]) : DeviceManagerUi.Instance.nullIconSprite;
            dragAndDropIcon.color = multiRae.Devices.ContainsKey("Tracked Device with Menu, Trigger, Grip Buttons") ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            dragAndDropLabel.text = multiRae.GetLabelForSlot("Tracked Device with Menu, Trigger, Grip Buttons");
            dragAndDropLabel.color = multiRae.Devices.ContainsKey("Tracked Device with Menu, Trigger, Grip Buttons") ? Color.white : new Color(1, 1, 1, 0.5f);
        }

        public void SetDevice(Device d)
        {
            multiRae.SetDevice("Tracked Device with Menu, Trigger, Grip Buttons", d);
        }
    }
}
