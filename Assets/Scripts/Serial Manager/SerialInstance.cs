using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialInstance : MonoBehaviour
{
    public SerialPort serialPort;
    public bool serialPortIsOpen;
    public int baudRate = -1;
    public int serialPortBytesToRead;

    public delegate void Message(string line);
    public event Message OnMessage;
    public List<SerialInterpreter> subscribers = new List<SerialInterpreter>();

    public List<string> messageLog = new List<string>();

    public void Open()
    {
        try
        {
            SerialPort sp = new SerialPort(name, baudRate, Parity.None, 8, StopBits.One);
            sp.DtrEnable = true;
            sp.RtsEnable = true;
            sp.ReadTimeout = 500;
            sp.WriteTimeout = 500;
            sp.Open();
            serialPort = sp;
            Debug.Log(string.Format("Serial port opened on {0} with baudrate {1}.", name, baudRate));
        }

        catch
        {
            Debug.Log(string.Format("Failed to open serial port on {0} with baudrate {1}.", name, baudRate));
        }
    }

    public void Close()
    {

        baudRate = -1;
        serialPort.Close();
    }

    private void Update()
    {
        serialPortIsOpen = false;
        if (serialPort != null)
            serialPortIsOpen = serialPort.IsOpen;

        if(serialPort != null)
        {
            if(serialPort.IsOpen)
            {
                serialPortBytesToRead = serialPort.BytesToRead;

                int currentIterations = 0;

                while(serialPort.BytesToRead > 0)
                {
                    try
                    {
                        string line = serialPort.ReadLine();
                        if (line.Length > 0)
                        {
                            OnMessage(line);
                            AddMessageToLog(line);
                        }

                        currentIterations++;
                        if (currentIterations >= 100)
                        {
                            serialPort.Close();
                            Debug.LogError("Couldn't read line from the serial stream! (Max iterations exceeded.) Closing stream.");
                            break;
                        }
                    }
                    catch
                    {
                        //serialPort.Close();
                        //Debug.LogError("Couldn't read line from the serial stream! (Exception caught.) Closing stream.");
                        break;
                    }
                }
            }
        }
    }

    private void AddMessageToLog(string message)
    {
        messageLog.Add(message);
        if (messageLog.Count > 10)
            messageLog.RemoveAt(0);
    }
}
