using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
public class UltrasoundDistanceComReader : MonoBehaviour {
    public static UltrasoundDistanceComReader instance;

    public enum Mode
    {
        Singles,
        Quadrants
    }

    public Mode mode = Mode.Singles;
    SerialPort sp;
    public  string comPort = "COM3";
    float lastTime = 0;
    public float disScale = 0.1f;
    private Dictionary<int, Obstical> AllObstacles = new Dictionary<int, Obstical>();
    public List<SensorInfo> sensorList = new List<SensorInfo>();
    public List<Quadrant> quadrantList = new List<Quadrant>();
    private Dictionary<int, SensorInfo> sensorMap = new Dictionary<int, SensorInfo>();
    private List<int> nullList = new List<int>();
    private string remainingData = "";

    // Use this for initialization
    void Start ()
    {
        instance = this;
        sp = new SerialPort();
        // sp.DataReceived += serialPort1_DataReceived;
        sp.PortName = comPort;
        sp.BaudRate = 9600;
        sp.DtrEnable = true;
        sp.Open();
    }
    private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
    {
        string line = sp.ReadLine();
    }

	// Update is called once per frame
	void Update ()
    {
        if (sp.IsOpen/* && Time.time - lastTime > 0.1f*/)
        {
            lastTime = Time.time;
            string line = "";
            while(sp.BytesToRead > 0)
            {
                line = sp.ReadLine();
            }
            if (line.Length >= 3)
            {
                AddLine(line);
            }

        }

        //if (Input.GetKey(KeyCode.Alpha1))
        //    AddLine("0 5 1 100 2 15 3 100");
        //if (Input.GetKey(KeyCode.Alpha2))
        //    AddLine("0 15 1 100 2 150 3 15");
        //if (Input.GetKey(KeyCode.Alpha3))
        //    AddLine("0 100 1 5 2 5 3 100");
        //if (Input.GetKey(KeyCode.Alpha4))
        //    AddLine("0 100 1 15 2 100 3 5");
    }

    public bool GetBoolCheck(int sensorId)
    {
        if (sensorMap.ContainsKey(sensorId))
            return sensorMap[sensorId].isDetected;
        else
            return false;
    }
    private void AddLine(string data)
    {
        if (data.Length > 0)
        {
            string[] split = data.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2 || split.Length % 2 != 0)
                return;
            for (int i = 0; i < split.Length; i += 2)
            {
                int index = int.Parse(split[i]);
                float distance = float.Parse(split[i+1]);

                if (AllObstacles.ContainsKey(index))
                {
                    Obstical obs = AllObstacles[index];
                    obs.visualIndicator.transform.position = obs.sensorIndicator.transform.position + (obs.sensorIndicator.transform.forward * distance * disScale);
                    if (sensorMap[index] != null)
                    {
                        sensorMap[index].lastDistance = distance;
                    }
                    if (mode == Mode.Quadrants)
                    {
                        for (int q = 0; q < quadrantList.Count; ++q)
                        {
                            bool quadrantTest = true;
                            for (int s = 0; s < sensorList.Count; ++s)
                            {
                                if (quadrantList[q].indexedAreaNum[s] != sensorList[s].detectedArea)
                                {
                                    quadrantTest = false;
                                    break;
                                }
                            }
                            if (quadrantTest)
                                quadrantList[q].quadrantObj.GetComponent<Renderer>().enabled = (true);
                            else
                                quadrantList[q].quadrantObj.GetComponent<Renderer>().enabled = (false);
                        }
                    }
                }
                else if (!nullList.Contains(index))
                {
                    GameObject obj = GameObject.Find("Sensor" + index);
                    if (obj == null)
                    {
                        nullList.Add(index);
                        return;
                    }
                    SensorInfo sensor = sensorList.Find((t) => t.sensorId == index);
                    if (sensor != null)
                    {
                        sensor.lastDistance = distance;
                        sensorMap.Add(index, sensor);
                    }
                    Obstical obs = new Obstical();
                    obs.sensorIndicator = obj;
                    obs.visualIndicator = GameObject.Instantiate(obs.sensorIndicator);
                    AllObstacles.Add(index, obs);
                    obs.visualIndicator.transform.SetParent(obs.sensorIndicator.transform);
                    obs.visualIndicator.transform.localPosition = new Vector3(0, 0, distance / 10);
                    Renderer renderer;
                    if ((renderer = obs.visualIndicator.GetComponent<Renderer>()) != null && mode != Mode.Singles)
                        renderer.enabled = false;
                    Random.InitState(index);
                    obs.visualIndicator.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
                }
            }
        }

    }

    private void OnDestroy()
    {
        if (sp.IsOpen)
            sp.Close();
    }

    public class Obstical
    {
        public GameObject visualIndicator, sensorIndicator;
        public int Angle;
        public int Distance;
        public List<int> LastDistances = new List<int>();

    }

    [System.Serializable]
    public class SensorInfo
    {
        public int sensorId;
        public bool isDetected { get => detectedArea >= 0; }
        public int detectedArea { get => Detection(); }
        public List<float> detectRange = new List<float>() { 0 };
        public float lastDistance = -1;

        int Detection()
        {
            if (detectRange.Count > 1)
            {
                for (int i = 1; i < detectRange.Count; ++i)
                {
                    if (detectRange[i - 1] <= lastDistance && detectRange[i] > lastDistance)
                        return i;
                }
            }
            return -1;
        }
    }

    [System.Serializable]
    public class Quadrant
    {
        public List<int> indexedAreaNum = new List<int>();
        public GameObject quadrantObj;
    }

}
