using Leap;
using NextGen.VrManager.Ui;
using TMPro;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Device = NextGen.VrManager.Devices.Device;
using System;
using UnityEngine;

namespace NextGen.Tools.Ui
{
    public class StaticHandUiWidget : ToolInspectorUiWidget
    {
        public StaticHand staticHand;

        public DragAndDropSlot dragAndDropSlot;
        private Image dragAndDropIcon;
        public TextMeshProUGUI dragAndDropLabel;

        public override Type ToolType => typeof(StaticHand);

        // Start is called before the first frame update
        void Start()
        {
            if (nextGenInspectorUi)
                staticHand = (StaticHand)nextGenInspectorUi.Tool;

            dragAndDropIcon = dragAndDropSlot.transform.GetChild(0).GetComponent<Image>();

            dragAndDropSlot.slotRequirements = staticHand.DeviceSlots["Controller"];
        }

        private void OnEnable()
        {
            dragAndDropSlot.deviceSet += SetDevice;
        }

        public void SetDevice(Device d)
        {
            staticHand.SetDevice("Controller", d);
        }

        // Update is called once per frame
        void Update()
        {
            dragAndDropIcon.sprite = staticHand.Devices.ContainsKey("Controller") ? DeviceManagerUi.GetIconSpriteForDevice(staticHand.Devices[("Controller")]) : DeviceManagerUi.Instance.nullIconSprite;
            dragAndDropIcon.color = staticHand.Devices.ContainsKey("Controller") ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            dragAndDropLabel.text = staticHand.GetLabelForSlot("Controller");
            dragAndDropLabel.color = staticHand.Devices.ContainsKey("Controller") ? Color.white : new Color(1, 1, 1, 0.5f);
        }
    }
}