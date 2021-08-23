using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ArduinoDetector : MonoBehaviour
{

    private Vector3 temp;

    private SerialPort stream = new SerialPort(@"\\.\" + "COM3", 9600);

    private bool _isEmpty = false;

    public bool isEmpty { get { return _isEmpty; } }

    // Use this for initialization
    void Start()
    {
        stream.Open();
        stream.ReadTimeout = 25;
        StartCoroutine(readString());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator readString()
    {

        while (true)
        {

            if (stream.IsOpen)
            {

                try
                {

                    string value = stream.ReadLine();
                    string[] values = value.Split(',');

                    float x = int.Parse(values[0]);
                    float y = -1 * (int.Parse(values[1]));
                    // Debug.Log("(x,y): " + x.ToString() + ", " + y.ToString());
                    _isEmpty = x > 200 && y < -240;
                    //if (x > 200 && y < -240)
                    //{
                    //    Debug.Log("Empty");
                    //}
                    //else
                    //{
                    //    x = x / 10f;
                    //    y = y / 10f;
                    //    temp = transform.position;
                    //    temp.x = x;
                    //    temp.y = y;
                    //    transform.position = temp;
                    //}

                }
                catch (System.Exception)
                {

                }

            }

            yield return null;

        }

    }
}