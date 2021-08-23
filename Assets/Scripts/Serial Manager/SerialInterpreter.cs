using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for serial interpreters like the TouchpadInterpreter or UltrasoundInterpreter.

public abstract class SerialInterpreter : MonoBehaviour
{
    [HideInInspector] public int baudRate;

    public SerialInstance serialInstance;

    public bool SetSerialInstance(SerialInstance _serialInstance)
    {
        if (serialInstance != null)
        {
            serialInstance.OnMessage -= ReceiveLine;
            serialInstance.subscribers.Remove(this);
        }

        //Port is closed.
        if (!_serialInstance.serialPortIsOpen)
        {
            _serialInstance.baudRate = baudRate;
            _serialInstance.Open();
            _serialInstance.subscribers.Add(this);

            serialInstance = _serialInstance;
        }
        //Port is already open.
        else
        {
            if(_serialInstance.baudRate != baudRate)
            {
                if (_serialInstance.subscribers.Count > 0)
                {
                    string errorString = "This serial instance currently has " + _serialInstance.subscribers.Count + " subscribers which run at a different baudrate: ";

                    foreach (SerialInterpreter subscriber in _serialInstance.subscribers)
                        errorString += subscriber.name + ". ";

                    errorString += "Deactivate them first.";
                    Debug.LogError(errorString);

                    return false;
                }
                else
                {
                    _serialInstance.Close();
                    _serialInstance.baudRate = baudRate;
                    _serialInstance.Open();
                    _serialInstance.subscribers.Add(this);

                    serialInstance = _serialInstance;
                }
            }
            else
            {
                _serialInstance.subscribers.Add(this);

                serialInstance = _serialInstance;
            }

        }

        serialInstance = _serialInstance;
        serialInstance.OnMessage += ReceiveLine;

        return true;
    }

    public void SetInactive()
    {
        if (serialInstance != null)
        {
            serialInstance.OnMessage -= ReceiveLine;
            serialInstance.subscribers.Remove(this);
        }

        serialInstance = null;
    }

    private void OnEnable()
    {
        if (serialInstance != null)
        {
            serialInstance.OnMessage += ReceiveLine;
            serialInstance.subscribers.Add(this);
        }
    }
    private void OnDisable()
    {
        if (serialInstance != null)
        {
            serialInstance.OnMessage -= ReceiveLine;
            serialInstance.subscribers.Remove(this);
        }
    }
    public abstract void ReceiveLine(string line);
}
