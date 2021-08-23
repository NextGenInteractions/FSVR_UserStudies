using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Linq;

public class HandDeviceArduino : MonoBehaviour
{
    public static HandDeviceArduino instance;
    SerialPort sp;
    public string comPort = "COM3";
    public int baudrate = 9600;
    public Image[] numpadList;
    public string[] numpadStrList;
    public RectTransform touchPadCursor;
    public Slider capacitivePos, capacitiveMag;

    Dictionary<string, Image> numpadMap = new Dictionary<string, Image>();
    Color hl = new Color(0, 1, 0, 0.5f), ori = new Color(1, 1, 1, 0.5f);
    Image lastHighlight;
    float touchPad_X_Unit = 50, touchPad_Y_Unit = 20;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        for(int i = 0; i < numpadList.Length; ++i)
        {
            numpadMap.Add(numpadStrList[i], numpadList[i]);
        }
        sp = new SerialPort(comPort, baudrate, Parity.None, 8, StopBits.One);
        sp.DtrEnable = true;
        sp.RtsEnable = true;
        sp.Open();
    }

    // Update is called once per frame
    void Update()
    {
        if (sp.IsOpen)
        {
            string line = "";
            while (sp.BytesToRead > 0)
                line = sp.ReadLine();
            if (line.Length > 0)
            {
                string[] split = line.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                    Decryption(s);
            }
        }
    }

    void Decryption(string s)
    {
        if (s.Length > 0)
        {
            string[] split = s.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
            switch (split[0].ToLower())
            {
                case "key":
                case "wheel":
                    if (lastHighlight != null)
                        lastHighlight.color = ori;
                    numpadMap[split[1]].color = hl;
                    lastHighlight = numpadMap[split[1]];
                    break;
                case "x":
                    float num = -1;
                    Vector3 pos = touchPadCursor.localPosition;
                    if (float.TryParse(split[1], out num) && num > -1)
                        pos.x = num * touchPad_X_Unit;
                    touchPadCursor.localPosition = pos;
                    break;
                case "y":
                    float y = -1;
                    Vector3 posy = touchPadCursor.localPosition;
                    if (float.TryParse(split[1], out y) && y > -1)
                        posy.y = y * touchPad_Y_Unit;
                    touchPadCursor.localPosition = posy;
                    break;
                case "pos":
                    float posCap = -1;
                    if (float.TryParse(split[1], out posCap) && posCap > -1)
                        capacitivePos.value = posCap;
                    break;
                case "mag":
                    float magCap = -1;
                    if (float.TryParse(split[1], out magCap) && magCap > -1)
                        capacitiveMag.value = magCap;
                    break;
            }
        }
    }
}
