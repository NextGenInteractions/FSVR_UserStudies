using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;

namespace NextGen.Tools
{
    public class MultiRae : Tool
    {
        public bool menuInput = false;
        private bool triggerInput = false;
        private bool gripInput = false;

        private bool lastMenuInput = false;
        private bool lastTriggerInput = false;
        private bool lastGripInput = false;

        public bool deviceOn = false;

        public float batteryLevel;

        public PlayMakerFSM fsmToTurnOn;

        [Header("Animated Offsets")]
        [SerializeField] private Transform rightButtonOffset;
        [SerializeField] private Transform leftButtonOffset;

        private void Awake()
        {
            _name = name;

            _deviceSlots =
            new Dictionary<string, DeviceSlot>() {
                {
                    "Tracked Device with Menu, Trigger, Grip Buttons",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked,
                            (DeviceFeatureUsage)CommonDeviceFeatures.triggerButton,
                            (DeviceFeatureUsage)CommonDeviceFeatures.gripButton,
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

        public void SetChildEnabled()
        {
            transform.GetChild(0).gameObject.SetActive(Devices != null && Devices.ContainsKey("Tracked Device with Menu, Trigger, Grip Buttons"));
        }

        public void GetPosAndRot()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Tracked Device with Menu, Trigger, Grip Buttons"))
                {
                    if (Devices["Tracked Device with Menu, Trigger, Grip Buttons"].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos))
                    {
                        if (getPos != Vector3.zero) //If getPos is 0, then tracking is lost and we want to disregard the tracked position until it's back.
                        {
                            transform.localPosition = getPos;
                            if (Devices["Tracked Device with Menu, Trigger, Grip Buttons"].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot))
                                transform.localRotation = getRot;
                        }
                    }
                }
            }
        }

        public void GetInput()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Tracked Device with Menu, Trigger, Grip Buttons"))
                {
                    //Menu button
                    if (Devices["Tracked Device with Menu, Trigger, Grip Buttons"].TryGetFeatureValue(CommonDeviceFeatures.menuButton, out bool getMenuButton))
                        menuInput = getMenuButton;

                    //Trigger button
                    if (Devices["Tracked Device with Menu, Trigger, Grip Buttons"].TryGetFeatureValue(CommonDeviceFeatures.triggerButton, out bool getTriggerButton))
                        triggerInput = getTriggerButton;

                    if (!lastMenuInput && menuInput)
                    {
                        if (logInputToMetricLog)
                            MetricManager.LogEvent("MultiRAE: Left Button pressed.", "Input");
                    }

                    if (!lastTriggerInput && triggerInput)
                    {
                        if (logInputToMetricLog)
                            MetricManager.LogEvent("MultiRAE: Right Button pressed.", "Input");

                        if(!deviceOn)
                            TurnOnDevice();
                    }

                    //Animate offsets based on button input.
                    if(rightButtonOffset != null)
                        rightButtonOffset.localPosition = triggerInput ? new Vector3(0, -0.001f, 0) : Vector3.zero;
                    if(leftButtonOffset != null)
                        leftButtonOffset.localPosition = menuInput ? new Vector3(0, -0.001f, 0) : Vector3.zero;

                    lastMenuInput = menuInput;
                    lastTriggerInput = triggerInput;
                }
            }
        }

        public void GetBatteryLevel()
        {
            if(Devices.ContainsKey("Tracked Device with Menu, Trigger, Grip Buttons"))
            {
                if (Devices["Tracked Device with Menu, Trigger, Grip Buttons"].TryGetFeatureValue(CommonDeviceFeatures.batteryLevel, out float outBatteryLevel))
                    batteryLevel = outBatteryLevel;
            }
        }

        void TurnOnDevice()
        {
            deviceOn = true;
            fsmToTurnOn.SendEvent("Turn on MultiRAE");
        }
    }
}
