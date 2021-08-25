using System;
using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.Tools.Ui
{
    public class SimpleTrackedObjectUiWidget : ToolInspectorUiWidget
    {
        public SimpleTrackedObject simpleTrackedObject;

        public DragAndDropSlot dragAndDropSlot;
        private Image dragAndDropIcon;
        public TextMeshProUGUI dragAndDropLabel;

        public override Type ToolType => typeof(SimpleTrackedObject);

        private void Start()
        {
            if (nextGenInspectorUi)
                simpleTrackedObject = (SimpleTrackedObject)nextGenInspectorUi.Tool;

            dragAndDropIcon = dragAndDropSlot.transform.GetChild(0).GetComponent<Image>();

            dragAndDropSlot.slotRequirements = simpleTrackedObject.DeviceSlots["Tracked Device"];
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            dragAndDropSlot.deviceSet += SetDevice;
        }

        // Update is called once per frame
        void Update()
        {
            dragAndDropIcon.sprite = simpleTrackedObject.Devices.ContainsKey("Tracked Device") ? DeviceManagerUi.GetIconSpriteForDevice(simpleTrackedObject.Devices[("Tracked Device")]) : DeviceManagerUi.Instance.nullIconSprite;
            dragAndDropIcon.color = simpleTrackedObject.Devices.ContainsKey("Tracked Device") ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            dragAndDropLabel.text = simpleTrackedObject.GetLabelForSlot("Tracked Device");
            dragAndDropLabel.color = simpleTrackedObject.Devices.ContainsKey("Tracked Device") ? Color.white : new Color(1,1,1,0.5f);
        }

        public void SetDevice(Device d)
        {
            simpleTrackedObject.SetDevice("Tracked Device", d);
        }
    }
}
