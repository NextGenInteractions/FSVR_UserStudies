using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class ButtonScript : MonoBehaviour
{
    private UduinoManager boardManager;

    public int ArduinoPin;
    public GameObject buttonLight;

    public bool buttonDown = false;

    // Start is called before the first frame update
    void Start()
    {
        boardManager = UduinoManager.Instance;
        boardManager.pinMode(ArduinoPin, PinMode.Input_pullup); //Green Button
    }

    // Update is called once per frame
    void Update()
    {
        buttonDown = boardManager.digitalRead(ArduinoPin) == 0;

        buttonLight.GetComponent<Light>().enabled = buttonDown;//SetActive(buttonDown);
    }

    public bool isButtonDown()
    {
        return buttonDown;

    }
}
