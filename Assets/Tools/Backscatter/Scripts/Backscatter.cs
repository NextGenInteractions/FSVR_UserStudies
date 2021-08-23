using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;
using HutongGames.PlayMaker;

namespace NextGen.Tools
{
    public class Backscatter : Tool
    {
        public Material emissiveBlue;
        public Material emissiveDarkBlue;

        public RenderTexture renderTexture;

        public Renderer xrayScan;
        public GameObject scanner;
        public Camera cam;

        public bool setting = false;

        private void Awake()
        {
            _name = "Backscatter";

            _deviceSlots =
            new Dictionary<string, DeviceSlot>() {
                {
                    "Backscatter",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked,
                            (DeviceFeatureUsage)CommonDeviceFeatures.primary2DAxisClick
                        }
                    }
                }
            };
        }

        // Update is called once per frame
        void Update()
        {
            SetChildEnabled();
            //Get tracking and input from paired tracked device.
            GetPosAndRot();
            GetInput();
        }

        public void Toggle(bool _setting)
        {
            setting = _setting;

            xrayScan.materials[2] = setting ? emissiveDarkBlue : emissiveBlue;
            scanner.SetActive(setting);

            cam.enabled = setting;
            cam.targetTexture = setting ? renderTexture : null;

            FsmBool policeXray = FsmVariables.GlobalVariables.FindFsmBool("ActiveScenario_Police_XrayScanning");
            policeXray.Value = setting;
        }

        public void SetChildEnabled()
        {
            transform.GetChild(0).gameObject.SetActive(Devices != null && Devices.ContainsKey("Backscatter"));
        }

        public void GetPosAndRot()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Backscatter"))
                {
                    if (Devices["Backscatter"].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos))
                    {
                        if (getPos != Vector3.zero) //If getPos is 0, then tracking is lost and we want to disregard the tracked position until it's back.
                        {
                            transform.position = getPos;
                            if (Devices["Backscatter"].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot))
                                transform.rotation = getRot;
                        }
                    }
                }
            }
        }

        public void GetInput()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Backscatter"))
                {
                    //Touchpad click
                    if (Devices["Backscatter"].TryGetFeatureValue(CommonDeviceFeatures.gripButton, out bool getGripButton))
                    {
                        if (setting != getGripButton)
                            Toggle(getGripButton);
                    }
                }
            }
        }
    }
}
