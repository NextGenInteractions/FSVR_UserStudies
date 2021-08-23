using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.Tools.Ui
{
    public class FlashlightUiWidget : ToolInspectorUiWidget
    {
        public Flashlight flashlight;

        public DragAndDropSlot dragAndDropSlot;
        private Image dragAndDropIcon;
        public TextMeshProUGUI dragAndDropLabel;

        public override Type ToolType => typeof(Flashlight);

        private void Start()
        {
            if (nextGenInspectorUi)
                flashlight = (Flashlight)nextGenInspectorUi.Tool;

            dragAndDropIcon = dragAndDropSlot.transform.GetChild(0).GetComponent<Image>();

            dragAndDropSlot.slotRequirements = flashlight.DeviceSlots[Flashlight.Tracked_Device_With_Trigger_Button_And_Grip_Button];
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            dragAndDropSlot.deviceSet += SetDevice;
        }

        // Update is called once per frame
        void Update()
        {
            var key = Flashlight.Tracked_Device_With_Trigger_Button_And_Grip_Button;
            dragAndDropIcon.sprite = flashlight.Devices.ContainsKey(key) ? DeviceManagerUi.GetIconSpriteForDevice(flashlight.Devices[key]) : DeviceManagerUi.Instance.nullIconSprite;
            dragAndDropIcon.color = flashlight.Devices.ContainsKey(key) ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            dragAndDropLabel.text = flashlight.GetLabelForSlot(key);
            dragAndDropLabel.color = flashlight.Devices.ContainsKey(key) ? Color.white : new Color(1,1,1,0.5f);
        }

        public void SetDevice(Device d)
        {
            flashlight.SetDevice(Flashlight.Tracked_Device_With_Trigger_Button_And_Grip_Button, d);
        }
    }
}
