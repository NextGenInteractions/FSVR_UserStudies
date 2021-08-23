using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

//Hello, world. (2)
public class SerialManager : MonoBehaviour
{
    public static SerialManager instance;

    public static SerialInstance[] SerialPorts
    {
        get
        {
            List<SerialInstance> serialPorts = new List<SerialInstance>();
            foreach (SerialInstance sp in instance.GetComponentsInChildren<SerialInstance>())
                serialPorts.Add(sp);
            return serialPorts.ToArray();
        }
    }

    public static SerialInstance FirstSerialInstance
    {
        get
        {
            SerialInstance[] sps = SerialPorts;
            if (sps.Length == 0)
            {
                return null;
            }
            else return sps[0];
        }
    }

    public static SerialInstance GetFirstSerialInstance()
    {
        return FirstSerialInstance;
    }

    public static SerialInstance GetSerialInstanceByName(string name)
    {
        //Debug.Log("Getting SPI by name: " + name + ".");

        SerialInstance toRet = null;

        for (int i = 0; i < instance.transform.childCount; i++)
        {
            if (instance.transform.GetChild(i).name == name)
            {
                toRet = instance.transform.GetChild(i).GetComponent<SerialInstance>();
                break;
            }
        }

        return toRet;
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        Refresh();
    }

    public static void Refresh()
    {
        instance.CloseSerials();
        instance.GetSerials();
    }

    public void GetSerials()
    {

        foreach (string name in SerialPort.GetPortNames())
        {
                SerialInstance instance = new GameObject(name).AddComponent<SerialInstance>();
                instance.transform.parent = transform;
        }
    }

    public void CloseSerials()
    {
        foreach (SerialInstance sp in GetComponentsInChildren<SerialInstance>())
        {
            if(sp.serialPort != null)
                if(sp.serialPort.IsOpen)
                    sp.serialPort.Close();
            Destroy(sp.gameObject);
        }
    }

    public static string[] GetNames()
    {
        string[] toRet = new string[instance.transform.childCount];

        for(int i = 0; i < instance.transform.childCount; i++)
        {
            toRet[i] = instance.transform.GetChild(i).name;
        }

        return toRet;
    }

    private void OnDisable()
    {
        Debug.Log("Closing all ports...");
        CloseSerials();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Refresh();
    }
}
