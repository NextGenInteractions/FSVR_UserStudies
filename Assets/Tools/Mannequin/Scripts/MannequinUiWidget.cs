using NextGen.VrManager.Devices;
using NextGen.VrManager.Ui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.Tools.Ui
{
    public class MannequinUiWidget : ToolInspectorUiWidget
    {
        public Mannequin mannequin;

        [Header("Head")]
        public DragAndDropSlot headSlot;
        private Image headIcon;
        public TextMeshProUGUI headLabel;

        [Header("Pelvis")]
        public DragAndDropSlot pelvisSlot;
        private Image pelvisIcon;
        public TextMeshProUGUI pelvisLabel;

        [Header("Left Arm")]
        public DragAndDropSlot leftArmSlot;
        private Image leftArmIcon;
        public TextMeshProUGUI leftArmLabel;

        [Header("Right Arm")]
        public DragAndDropSlot rightArmSlot;
        private Image rightArmIcon;
        public TextMeshProUGUI rightArmLabel;

        [Header("Left Leg")]
        public DragAndDropSlot leftLegSlot;
        private Image leftLegIcon;
        public TextMeshProUGUI leftLegLabel;

        [Header("Right Leg")]
        public DragAndDropSlot rightLegSlot;
        private Image rightLegIcon;
        public TextMeshProUGUI rightLegLabel;

        public override Type ToolType => typeof(Mannequin);

        private void Start()
        {
            if (nextGenInspectorUi)
                mannequin = (Mannequin)nextGenInspectorUi.Tool;

            headIcon = headSlot.transform.GetChild(0).GetComponent<Image>();
            pelvisIcon = pelvisSlot.transform.GetChild(0).GetComponent<Image>();
            leftArmIcon = leftArmSlot.transform.GetChild(0).GetComponent<Image>();
            rightArmIcon = rightArmSlot.transform.GetChild(0).GetComponent<Image>();
            leftLegIcon = leftLegSlot.transform.GetChild(0).GetComponent<Image>();
            rightLegIcon = rightLegSlot.transform.GetChild(0).GetComponent<Image>();

            headSlot.slotRequirements = mannequin.DeviceSlots["Head"];
            pelvisSlot.slotRequirements = mannequin.DeviceSlots["Pelvis"];
            leftArmSlot.slotRequirements = mannequin.DeviceSlots["Left Arm"];
            rightArmSlot.slotRequirements = mannequin.DeviceSlots["Right Arm"];
            leftLegSlot.slotRequirements = mannequin.DeviceSlots["Left Leg"];
            rightLegSlot.slotRequirements = mannequin.DeviceSlots["Right Leg"];
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            headSlot.deviceSet += SetHead;
            pelvisSlot.deviceSet += SetPelvis;
            leftArmSlot.deviceSet += SetLeftArm;
            rightArmSlot.deviceSet += SetRightArm;
            leftLegSlot.deviceSet += SetLeftLeg;
            rightLegSlot.deviceSet += SetRightLeg;
        }

        // Update is called once per frame
        void Update()
        {
            SetIconAndLabel(headIcon, headLabel, "Head");
            SetIconAndLabel(pelvisIcon, pelvisLabel, "Pelvis");
            SetIconAndLabel(leftArmIcon, leftArmLabel, "Left Arm");
            SetIconAndLabel(rightArmIcon, rightArmLabel, "Right Arm");
            SetIconAndLabel(leftLegIcon, leftLegLabel, "Left Leg");
            SetIconAndLabel(rightLegIcon, rightLegLabel, "Right Leg");
        }

        public void SetIconAndLabel(Image icon, TextMeshProUGUI label, string key)
        {
            icon.sprite = mannequin.Devices.ContainsKey(key) ? DeviceManagerUi.GetIconSpriteForDevice(mannequin.Devices[(key)]) : DeviceManagerUi.Instance.nullIconSprite;
            icon.color = mannequin.Devices.ContainsKey(key) ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 16);
            label.text = mannequin.GetLabelForSlot(key);
            label.color = mannequin.Devices.ContainsKey(key) ? Color.white : new Color(1, 1, 1, 0.5f);
        }

        public void SetHead(Device d)
        {
            mannequin.SetDevice("Head", d);
        }
        public void SetPelvis(Device d)
        {
            mannequin.SetDevice("Pelvis", d);
        }
        public void SetLeftArm(Device d)
        {
            mannequin.SetDevice("Left Arm", d);
        }
        public void SetRightArm(Device d)
        {
            mannequin.SetDevice("Right Arm", d);
        }
        public void SetLeftLeg(Device d)
        {
            mannequin.SetDevice("Left Leg", d);
        }
        public void SetRightLeg(Device d)
        {
            mannequin.SetDevice("Right Leg", d);
        }
    }
}
