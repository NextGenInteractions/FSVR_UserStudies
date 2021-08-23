using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;
using HutongGames.PlayMaker;

namespace NextGen.Tools
{
    public class Flashlight : Tool
    {
        public static readonly string Tracked_Device_With_Trigger_Button_And_Grip_Button = "Tracked Device with Trigger Button and Grip Button";

        private bool triggerInput = false;
        private bool gripInput = false;

        private bool lastTriggerInput = false;
        private bool lastGripInput = false;

        public GameObject flashlightRay;
        public GameObject bioscannerRay;

        public FlashlightState state;
        public FlashlightState State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                flashlightRay.SetActive(state == FlashlightState.Flashlight);
                bioscannerRay.SetActive(state == FlashlightState.Bioscanner);
            }
        }
        public enum FlashlightState
        {
            Off,
            Flashlight,
            Bioscanner
        }

        [Header("IK Targets")]
        [SerializeField] private Transform flashlightButtonIkTargetMount;
        [SerializeField] private Transform scannerButtonIkTargetMount;
        [SerializeField] private Transform thumbIkTarget;

        [Header("Animated Transforms")]
        [SerializeField] private Transform flashlightButtonOffset;
        [SerializeField] private Transform scannerButtonOffset;

        private void Awake()
        {
            _name = "Flashlight";

            _deviceSlots =
            new Dictionary<string, DeviceSlot>() {
                {
                    Tracked_Device_With_Trigger_Button_And_Grip_Button,
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
            transform.GetChild(0).gameObject.SetActive(Devices != null && Devices.ContainsKey(Tracked_Device_With_Trigger_Button_And_Grip_Button));
        }

        public void GetPosAndRot()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey(Tracked_Device_With_Trigger_Button_And_Grip_Button))
                {
                    if (Devices[Tracked_Device_With_Trigger_Button_And_Grip_Button].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos))
                    {
                        if (getPos != Vector3.zero) //If getPos is 0, then tracking is lost and we want to disregard the tracked position until it's back.
                        {
                            transform.position = getPos;
                            if (Devices[Tracked_Device_With_Trigger_Button_And_Grip_Button].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot))
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
                if (Devices.ContainsKey(Tracked_Device_With_Trigger_Button_And_Grip_Button))
                {
                    //Trigger button
                    if (Devices[Tracked_Device_With_Trigger_Button_And_Grip_Button].TryGetFeatureValue(CommonDeviceFeatures.triggerButton, out bool getTriggerButton))
                        triggerInput = getTriggerButton;

                    //Grip button
                    if (Devices[Tracked_Device_With_Trigger_Button_And_Grip_Button].TryGetFeatureValue(CommonDeviceFeatures.gripButton, out bool getGripButton))
                        gripInput = getGripButton;

                    //Animate offsets based on button inputs
                    flashlightButtonOffset.localPosition = triggerInput ? new Vector3(0, 0, -0.001f) : Vector3.zero;
                    scannerButtonOffset.localPosition = gripInput ? new Vector3(0, 0, -0.001f) : Vector3.zero;

                    //Change thumb IK target position based on button inputs
                    if (triggerInput & !gripInput)
                        thumbIkTarget.SetParent(flashlightButtonIkTargetMount);
                    if (!triggerInput & !gripInput)
                        thumbIkTarget.SetParent(scannerButtonIkTargetMount);
                    thumbIkTarget.localPosition = Vector3.zero;

                    //If trigger has been pressed down this frame...
                    if (triggerInput && !lastTriggerInput)
                    {
                        State = State == FlashlightState.Flashlight ? FlashlightState.Off : FlashlightState.Flashlight;
                    }

                    //If grip has been pressed down this frame...
                    if(gripInput && !lastGripInput)
                    {
                        State = State == FlashlightState.Bioscanner ? FlashlightState.Off : FlashlightState.Bioscanner;
                    }

                    FsmVariables.GlobalVariables.FindFsmBool("ActiveScenario_Police_FlashlightOn").Value = State == FlashlightState.Flashlight;
                    FsmVariables.GlobalVariables.FindFsmBool("ActiveScenario_Police_ScannerOn").Value = State == FlashlightState.Bioscanner;

                    lastTriggerInput = triggerInput;
                    lastGripInput = gripInput;
                }
            }
        }
    }
}
