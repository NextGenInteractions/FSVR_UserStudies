using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Linq;
using System.Globalization;
using UnityEngine.Events;
using System;

public class ArduinoInputHandler : MonoBehaviour
{

    public SerialInstance serialPortInstance;
    public SerialPort Sp
    {
        get
        {
            if (serialPortInstance != null) return serialPortInstance.serialPort;
            else return null;
        }
    }

    private TouchPadHandler touchPadHandler;

    public Action onUntouch;
    public Action onTouch;
    [Tooltip("Amount of time to detect untouch event")]
    public float untouchDetectionInterval = 0.1f;
    private float currentUntouchDetection = 0.1f;
    private int bytesReceivingInterval = 0;

    private void Start()
    {
        serialPortInstance = SerialManager.FirstSerialInstance;
        touchPadHandler = GetComponent<TouchPadHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Sp != null)
        {
            if (Sp.IsOpen)
            {
                if ((currentUntouchDetection -= Time.deltaTime) <= 0)
                {
                    /* Need revision Later
                     * If there's more Arduino input
                     * For multiple input device, we need individual detection of untouch event
                     */
                    if (bytesReceivingInterval <= 0 && onUntouch != null)
                        onUntouch.Invoke();
                    currentUntouchDetection = untouchDetectionInterval;
                    bytesReceivingInterval = 0;
                }
                bytesReceivingInterval += Sp.BytesToRead;
                string line = "";
                while (Sp.BytesToRead > 0)
                {
                    /* Need revision Later
                     * If there's more Arduino input
                     * Currently it goes through all data on the stack of the stream
                     * For others, it only needs the latest one at the moment of an update
                     */
                    line = Sp.ReadLine();
                    if (line.Length > 0)
                    {
                        string[] split = line.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in split)
                            Decryption(s);
                    }
                }
            }
        }
    }

    void Decryption(string s)
    {
        if (s.Length > 0)
        {
            string[] split = s.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
            float num = -1;
            switch (split[0].ToLower())
            {
                case "key":
                case "wheel":
                    /* Need a new handler for NumPad like Touchpad
                     * 
                    if (lastHighlight != null)
                        lastHighlight.color = ori;
                    numpadMap[split[1]].color = hl;
                    lastHighlight = numpadMap[split[1]];
                    */
                    break;
                case "y":
                    /* Note that X and Y value from Arduino is actually reversed in Unity
                     * Keep in mind if there's any update in the future
                     */
                    num = -1;
                    if (touchPadHandler != null && float.TryParse(split[1], out num) && num > -1)
                    {
                        touchPadHandler.UpdateCoord(num, null);
                    }
                    break;
                case "x":
                    num = -1;
                    if (touchPadHandler != null && float.TryParse(split[1], out num) && num > -1)
                    {
                        touchPadHandler.UpdateCoord(null, num);
                    }
                    break;
                case "pos":
                    /* Need a new handler for stripe like Touchpad
                     * 
                    num = -1;
                    if (float.TryParse(split[1], out num) && num > -1)
                        capacitivePos.value = num;
                    */
                    break;
                case "mag":
                    /* Need a new handler for stripe like Touchpad
                     * 
                    num = -1;
                    if (float.TryParse(split[1], out num) && num > -1)
                        capacitiveMag.value = num;
                    */
                    break;
            }
            if (onTouch != null)
                onTouch.Invoke();
        }
    }
}
