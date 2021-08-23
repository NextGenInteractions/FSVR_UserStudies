using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class FlexSensorInput : MonoBehaviour
{

    private UduinoManager boardManager;
    private int flexValue;

    public AnalogPin pinNumber;

    // Start is called before the first frame update
    void Start()
    {
        boardManager = UduinoManager.Instance;
        boardManager.pinMode(pinNumber, PinMode.Input); // Flex Sensor
    }

    // Update is called once per frame
    void Update()
    {
        flexValue = boardManager.analogRead(pinNumber);
    }

    public float getFlexValue()
    {
        return flexValue;
    }

}
