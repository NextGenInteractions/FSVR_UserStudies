using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NextGen.VrManager.Devices;
using System;

namespace NextGen.VrManager.Ui
{
    public class SetMetadataDialog : Dialog
    {
        private Device device;

        public TextMeshProUGUI dialogLabelText;
        public TextMeshProUGUI messageText;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI serialText;
        public TMP_InputField labelInputField;
        public TMP_Dropdown typeDropdown;

        public void Init(Device d)
        {
            device = d;

            nameText.text = device.Name;
            serialText.text = device.Uid;
        }

        public override void Start()
        {
            base.Start();

            GetComponentInChildren<Button>().onClick.AddListener(AttemptToSet);

            PopulateTypeDropdown();
            GuessDropdownDefault();
        }

        private void PopulateTypeDropdown()
        {
            typeDropdown.ClearOptions();
            typeDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(Devices.DeviceType))));
        }

        private void GuessDropdownDefault()
        {
            if (device.Name.Contains("Reference"))
                typeDropdown.value = (int)Devices.DeviceType.Lighthouse;
            else if (device.Name.Contains("Headset"))
                typeDropdown.value = (int)Devices.DeviceType.ViveHmd;
            else if (device.Name.Contains("Controller"))
                typeDropdown.value = (int)Devices.DeviceType.Wand;
            else if (device.Name.Contains("Tracker"))
                typeDropdown.value = (int)Devices.DeviceType.Puck;
            else if (device.Name.Contains("Serial"))
                typeDropdown.value = (int)Devices.DeviceType.Serial;
        }

        private void AttemptToSet()
        {
            if(labelInputField.text != "")
            {
                DeviceMetadataManager.SetMetadata(device, new DeviceMetadata(labelInputField.text, typeDropdown.value, new List<string>()));
                Destroy(gameObject);
            }
        }

        public void SetMessage(string message)
        {
            messageText.text = message;
        }
    }
}
