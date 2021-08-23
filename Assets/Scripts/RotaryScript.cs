using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class RotaryScript : MonoBehaviour
{
    private UduinoManager boardManager;
    private float encoderAngle;

    public Transform dial;
    // Start is called before the first frame update
    void Start()
    {
        boardManager = UduinoManager.Instance;

        //Delagate
        boardManager.OnDataReceived += GetRotationData; // Rotatary Encoder Information Read (Use Pin number 5 & 6 on the arduino. Refer ArduinoAdv.ino script.)
        
    }

    void GetRotationData(string value, UduinoDevice board)
    {
        
        if (value.StartsWith("e"))
        {
            encoderAngle = float.Parse(value.Remove(0, value.IndexOf(' ') + 1));

            dial.transform.localRotation = Quaternion.Euler(encoderAngle * 360.0f / 96.0f, 0.0f, 0.0f); // divided by 8 for rotation scaling
            Debug.Log(encoderAngle);
        }
    }
    public float getRotationValue()
    {
        return encoderAngle;
    }
}
