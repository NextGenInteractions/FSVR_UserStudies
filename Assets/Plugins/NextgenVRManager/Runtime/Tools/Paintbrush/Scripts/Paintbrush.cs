using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;

namespace NextGen.Tools
{
    public class Paintbrush : Tool
    {
        public float strokeWidth;
        public Color paintColor;
        private int paintColorInt = 0;
        public int PaintColorInt
        {
            get
            {
                return paintColorInt;
            }

            set
            {
                paintColorInt = value;
                switch(paintColorInt)
                {
                    case 0:
                        paintColor = Color.red;
                        break;

                    case 1:
                        paintColor = Color.blue;
                        break;

                    case 2:
                        paintColor = Color.green;
                        break;

                    case 3:
                        paintColor = Color.cyan;
                        break;

                    case 4:
                        paintColor = Color.magenta;
                        break;

                    case 5:
                        paintColor = Color.yellow;
                        break;
                }

                EndPaint();
            }
        } 

        public bool inputTrue;
        public bool clickTrue = false;
        public bool lastClickTrue = false;

        public float distanceMinimum = 0.025f;
        private Vector3 lastPaintPos;
        private PaintbrushStroke currentStroke;
        private int blobCount = 0;
        public Material strokeMaterial;

        public bool forcePaint;

        private void Awake()
        {
            _name = "Paintbrush";

            _deviceSlots =
            new Dictionary<string, DeviceSlot>() {
                {
                    "Tracked Device",
                    new DeviceSlot() {
                        RequiredCharacteristics = DeviceCharacteristics.TrackedDevice,
                        RequiredFeatures = new List<DeviceFeatureUsage>() {
                            (DeviceFeatureUsage)CommonDeviceFeatures.isTracked
                        }
                    }
                }
            };

            transform.GetChild(0).GetComponent<Renderer>().material = strokeMaterial;
        }

        private bool Painting { get { return inputTrue || forcePaint; } }

        // Update is called once per frame
        void Update()
        {
            transform.GetChild(0).GetComponent<Renderer>().material.color = paintColor;

            //Get tracking and (optionally) input from paired tracked device.
            GetPosAndRot();
            GetOptionalInput();
            
            //Paintbrush logic.
            if(Painting)
            {
                if(currentStroke == null) //Start a new stroke.
                {
                    StartPaint();
                }
                else //Continue painting the current stroke.
                {
                    if (Vector3.Distance(lastPaintPos, transform.GetChild(0).position) > distanceMinimum)
                    {
                        currentStroke.AddPos(transform.GetChild(0).position);
                        lastPaintPos = transform.GetChild(0).position;
                    }
                }
            }
            else
            {
                if(currentStroke != null)
                {
                    EndPaint();
                }
            }
        }

        void StartPaint()
        {
            GameObject stroke = new GameObject("Paintbrush Stroke " + ++blobCount);
            currentStroke = stroke.AddComponent<PaintbrushStroke>();
            currentStroke.Init(transform.GetChild(0).position, strokeMaterial, paintColor, strokeWidth);
            lastPaintPos = transform.GetChild(0).position;
            stroke.transform.SetParent(transform.parent);
        }

        void EndPaint()
        {
            currentStroke = null;
        }

        public void ToggleToNextPaintColor()
        {
            PaintColorInt += 1;
            if (PaintColorInt > 5)
                PaintColorInt = 0;
        }

        public void GetPosAndRot()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Tracked Device"))
                {
                    if (Devices["Tracked Device"].TryGetFeatureValue(CommonDeviceFeatures.devicePosition, out Vector3 getPos))
                    {
                        if (getPos != Vector3.zero) //If getPos is 0, then tracking is lost and we want to disregard the tracked position until it's back.
                        {
                            SetPosition(getPos);
                            if (Devices["Tracked Device"].TryGetFeatureValue(CommonDeviceFeatures.deviceRotation, out Quaternion getRot))
                                SetRotation(getRot);
                        }
                    }
                }
            }
        }

        public void GetOptionalInput()
        {
            if (Devices != null)
            {
                if (Devices.ContainsKey("Tracked Device"))
                {
                    //Trigger (optional)
                    if (Devices["Tracked Device"].TryGetFeatureValue(CommonDeviceFeatures.trigger, out float getTrigger))
                        inputTrue = getTrigger > 0.5f;

                    //Click (optional)
                    if (Devices["Tracked Device"].TryGetFeatureValue(CommonDeviceFeatures.primary2DAxisClick, out bool getClick))
                        clickTrue = getClick;
                    if (clickTrue && !lastClickTrue)
                        ToggleToNextPaintColor();
                    lastClickTrue = clickTrue;
                }
            }
        }
    }
}
