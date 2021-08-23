using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialPortInstance : MonoBehaviour
{
    public SerialPort serialPort;

    public delegate void Message(string line);
    public event Message OnMessage;

    private void Update()
    {
        if(serialPort.IsOpen)
        {
            while (serialPort.BytesToRead > 0)
            {
                string line = serialPort.ReadLine();
                if (line.Length > 0)
                {
                    string[] split = line.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in split)
                    {
                        OnMessage(s);
                    }
                }
            }
        }
    }
}
