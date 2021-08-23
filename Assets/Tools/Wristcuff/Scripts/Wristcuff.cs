using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;
using NextGen.VrManager.Devices.Serials;

namespace NextGen.Tools
{
    public class Wristcuff : Tool
    {
        public bool flipTouchpad;
        public EmsUiManager emsUiManager;

        private bool lastMenuButton;
        private bool lastTriggerButton;
        private bool lastGripButton;
        private bool lastPrimary2DAxisTouch;


        [Header("Snap Hands -- Ik Targets")]
        [SerializeField] private Transform indexFingerIkTarget;
        [SerializeField] private Transform auxButtonIkTarget;
        [SerializeField] private Transform trakButtonIkTarget;
        [SerializeField] private Transform aboveScreenIkTargetMount;
        [SerializeField] private Transform onTouchpointIkTargetMount;

        private void Awake()
        {
            _name = "Wristcuff";

            _deviceSlots =
            new Dictionary<string, DeviceSlot>() {
                {
                    "Tracked Device with Buttons",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                },
                {
                    "Touchpad",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.None,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)SerialDeviceFeatures.touchpad,
                            (DeviceFeatureUsage)SerialDeviceFeatures.touchpadTouch
                        }
                    }
                }
            };
        }

        public bool Touching
        {
            get
            {
                if(Devices.ContainsKey("Touchpad"))
                {
                    Devices["Touchpad"].TryGetFeatureValue(SerialDeviceFeatures.touchpadTouch, out bool getTouching);
                    return getTouching;
                }
                else
                {
                    return false;
                }
            }
        }
        public Vector2 Point {
            get
            {
                if (Devices.ContainsKey("Touchpad"))
                {
                    Devices["Touchpad"].TryGetFeatureValue(SerialDeviceFeatures.touchpad, out Vector2 getTouchpoint);
                    if (flipTouchpad)
                        getTouchpoint = new Vector2(14 - getTouchpoint.x, 10 - getTouchpoint.y);
                    return getTouchpoint;
                }
                else
                    return default;
            }
        }

        // Update is called once per frame
        void Update()
        {
            SetChildEnabled();
            //Get tracking and input from paired tracked device.
            GetPosAndRot();
            GetInput();

            //Animate the "index finger" IK target for snap hands.
            if(Touching) //If touching the touchpad, set the IK target to the touchpoint cursor.
                indexFingerIkTarget.SetParent(onTouchpointIkTargetMount);
            else //Otherwise, set it to a position floating in the air above the touchpad.
                indexFingerIkTarget.SetParent(aboveScreenIkTargetMount);
            indexFingerIkTarget.localPosition = Vector3.zero;
        }

        public void SetChildEnabled()
        {
            transform.GetChild(0).gameObject.SetActive(Devices != null && Devices.ContainsKey("Tracked Device with Buttons") && Devices.ContainsKey("Touchpad"));
        }

        public void GetPosAndRot()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Tracked Device with Buttons"))
                {
                    if (Devices["Tracked Device with Buttons"].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos))
                    {
                        if (getPos != Vector3.zero) //If getPos is 0, then tracking is lost and we want to disregard the tracked position until it's back.
                        {
                            transform.position = getPos;
                            if (Devices["Tracked Device with Buttons"].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot))
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
                if (Devices.ContainsKey("Tracked Device with Buttons"))
                {
                    //Get raw input.
                    Devices["Tracked Device with Buttons"].TryGetFeatureValue(CommonDeviceFeatures.menuButton, out bool getMenuButton);
                    Devices["Tracked Device with Buttons"].TryGetFeatureValue(CommonDeviceFeatures.triggerButton, out bool getTriggerButton);
                    Devices["Tracked Device with Buttons"].TryGetFeatureValue(CommonDeviceFeatures.gripButton, out bool getGripButton);
                    Devices["Tracked Device with Buttons"].TryGetFeatureValue(CommonDeviceFeatures.primary2DAxisTouch, out bool getPrimary2DAxisTouch);

                    //Generate "just pressed" events by checking this frame's raw input against "last frame" values.
                    if (getTriggerButton && !lastTriggerButton)
                        emsUiManager.SteamVrConfirm();
                    if (getPrimary2DAxisTouch && !lastPrimary2DAxisTouch)
                        emsUiManager.SteamVrBack();

                    //Animate snap hand IK targets using raw input values.
                    auxButtonIkTarget.localPosition = getMenuButton ? new Vector3(0, -0.5f, 0) : Vector3.zero;
                    trakButtonIkTarget.localPosition = getGripButton ? new Vector3(0, -0.5f, 0) : Vector3.zero;

                    //Set "last frame" values for next frame.
                    lastMenuButton = getMenuButton;
                    lastTriggerButton = getTriggerButton;
                    lastGripButton = getGripButton;
                    lastPrimary2DAxisTouch = getPrimary2DAxisTouch;
                }
            }
        }
    }
}
