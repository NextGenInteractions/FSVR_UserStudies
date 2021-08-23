using Leap;
using NextGen.VrManager.Ui;
using TMPro;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Device = NextGen.VrManager.Devices.Device;
using System;

namespace NextGen.Tools.Ui
{
    public class SnapHandUiWidget : ToolInspectorUiWidget
    {
        public SnapHand snapHand;

        public DragAndDropSlot dragAndDropSlot;
        private Image dragAndDropIcon;
        public TextMeshProUGUI dragAndDropLabel;

        public override Type ToolType => typeof(SnapHand);

        // Start is called before the first frame update
        void Start()
        {
            if (nextGenInspectorUi)
                snapHand = (SnapHand)nextGenInspectorUi.Tool;

            dragAndDropIcon = dragAndDropSlot.transform.GetChild(0).GetComponent<Image>();

            dragAndDropSlot.slotRequirements = snapHand.DeviceSlots["Hand"];
        }

        private void OnEnable()
        {
            dragAndDropSlot.deviceSet += SetDevice;
        }

        public void SetDevice(Device d)
        {
            snapHand.SetDevice("Hand", d);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}