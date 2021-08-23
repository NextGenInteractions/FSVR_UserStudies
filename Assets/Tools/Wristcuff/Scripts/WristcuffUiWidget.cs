using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.Tools.Ui
{
    public class WristcuffUiWidget : ToolInspectorUiWidget
    {
        public Wristcuff wristcuff;

        public DragAndDropSlot trackedDragAndDropSlot;
        private Image trackedDragAndDropIcon;
        public TextMeshProUGUI trackedDragAndDropLabel;

        public DragAndDropSlot serialDragAndDropSlot;
        private Image serialDragAndDropIcon;
        public TextMeshProUGUI serialDragAndDropLabel;

        public override Type ToolType => typeof(Wristcuff);

        private void Start()
        {
            if (nextGenInspectorUi)
                wristcuff = (Wristcuff)nextGenInspectorUi.Tool;

            trackedDragAndDropIcon = trackedDragAndDropSlot.transform.GetChild(0).GetComponent<Image>();
            serialDragAndDropIcon = serialDragAndDropSlot.transform.GetChild(0).GetComponent<Image>();

            trackedDragAndDropSlot.slotRequirements = wristcuff.DeviceSlots["Tracked Device with Buttons"];
            serialDragAndDropSlot.slotRequirements = wristcuff.DeviceSlots["Touchpad"];
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            trackedDragAndDropSlot.deviceSet += SetTrackedDevice;
            serialDragAndDropSlot.deviceSet += SetSerialDevice;
        }

        // Update is called once per frame
        void Update()
        {
            trackedDragAndDropIcon.sprite = wristcuff.Devices.ContainsKey("Tracked Device with Buttons") ? DeviceManagerUi.GetIconSpriteForDevice(wristcuff.Devices[("Tracked Device with Buttons")]) : DeviceManagerUi.Instance.nullIconSprite;
            trackedDragAndDropIcon.color = wristcuff.Devices.ContainsKey("Tracked Device with Buttons") ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            trackedDragAndDropLabel.text = wristcuff.GetLabelForSlot("Tracked Device with Buttons");
            trackedDragAndDropLabel.color = wristcuff.Devices.ContainsKey("Tracked Device with Buttons") ? Color.white : new Color(1, 1, 1, 0.5f);

            serialDragAndDropIcon.sprite = wristcuff.Devices.ContainsKey("Touchpad") ? DeviceManagerUi.GetIconSpriteForDevice(wristcuff.Devices[("Touchpad")]) : DeviceManagerUi.Instance.nullIconSprite;
            serialDragAndDropIcon.color = wristcuff.Devices.ContainsKey("Touchpad") ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            serialDragAndDropLabel.text = wristcuff.Devices.ContainsKey("Touchpad") ? wristcuff.Devices[("Touchpad")].DisplayName : "Touchpad";
            serialDragAndDropLabel.color = wristcuff.Devices.ContainsKey("Touchpad") ? Color.white : new Color(1, 1, 1, 0.5f);
        }

        public void SetTrackedDevice(Device d)
        {
            wristcuff.SetDevice("Tracked Device with Buttons", d);
        }

        public void SetSerialDevice(Device d)
        {
            wristcuff.SetDevice("Touchpad", d);
        }
    }
}
