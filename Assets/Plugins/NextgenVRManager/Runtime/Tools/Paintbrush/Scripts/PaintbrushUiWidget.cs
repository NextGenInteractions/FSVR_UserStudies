using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.Tools.Ui
{
    public class PaintbrushUiWidget : ToolInspectorUiWidget
    {
        public Paintbrush paintbrush;

        public DragAndDropSlot dragAndDropSlot;
        private Image dragAndDropIcon;
        public TextMeshProUGUI dragAndDropLabel;
        public Transform colorPickerRow;
        public Togglebox forcePaintTogglebox;

        public override Type ToolType => typeof(Paintbrush);

        private void Start()
        {
            if (nextGenInspectorUi)
                paintbrush = (Paintbrush)nextGenInspectorUi.Tool;

            dragAndDropIcon = dragAndDropSlot.transform.GetChild(0).GetComponent<Image>();

            dragAndDropSlot.slotRequirements = paintbrush.DeviceSlots["Tracked Device"];

            forcePaintTogglebox.state = paintbrush.forcePaint;
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            dragAndDropSlot.deviceSet += SetDevice;
        }

        // Update is called once per frame
        void Update()
        {
            dragAndDropIcon.sprite = paintbrush.Devices.ContainsKey("Tracked Device") ? DeviceManagerUi.GetIconSpriteForDevice(paintbrush.Devices[("Tracked Device")]) : DeviceManagerUi.Instance.nullIconSprite;
            dragAndDropIcon.color = paintbrush.Devices.ContainsKey("Tracked Device") ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            dragAndDropLabel.text = paintbrush.GetLabelForSlot("Tracked Device");
            dragAndDropLabel.color = paintbrush.Devices.ContainsKey("Tracked Device") ? Color.white : new Color(1,1,1,0.5f);

            for (int i = 0; i < colorPickerRow.childCount; i++)
            {
                colorPickerRow.GetChild(i).GetChild(0).GetComponent<Image>().enabled = paintbrush.PaintColorInt == i;
            }

            paintbrush.forcePaint = forcePaintTogglebox.state;
        }

        public void SetDevice(Device d)
        {
            paintbrush.SetDevice("Tracked Device", d);
        }

        public void SetPaintbrushColor(int color)
        {
            paintbrush.PaintColorInt = color;
        }

    }
}
