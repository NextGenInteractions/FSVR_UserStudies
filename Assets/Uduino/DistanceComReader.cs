using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
public class DistanceComReader : MonoBehaviour {
    public static DistanceComReader instance;
    SerialPort sp;
    public  string comPort = "COM3";
    float lastTime = 0;
    public float disScale = 0.1f;
    private Dictionary<int, Obstical> AllObstacles = new Dictionary<int, Obstical>();
    public List<SensorInfo> sensorList = new List<SensorInfo>();
    private Dictionary<int, SensorInfo> sensorMap = new Dictionary<int, SensorInfo>();
    private List<int> nullList = new List<int>();

    // Use this for initialization
    void Start ()
    {
        instance = this;
        sp = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
        sp.DtrEnable = true;
        sp.RtsEnable = true;
        sp.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        sp.Open();
        InvokeRepeating("ReadLine", 0.1f, 0.125f);
    }
    private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
    {
        // string line = sp.ReadLine();
        // Debug.Log(sender + e.ToString());
    }

	// Update is called once per frame
	void Update ()
    {
        //if (sp.IsOpen/* && Time.time - lastTime > 0.1f*/)
        //{
        //    lastTime = Time.time;
        //    string line = sp.ReadLine();
        //    Debug.Log(line);
        //    // AddLine(line);
        //}
    }
    
    void ReadLine()
    {
        if (sp.IsOpen/* && Time.time - lastTime > 0.1f*/)
        {
            lastTime = Time.time;
            if (sp.ReadExisting().Length >= 3)
            {
                string line = sp.ReadLine();
                Debug.Log(line);
                return;
                if (line.Length > 2)
                    AddLine(line);
            }
        }
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
            if (split.Length % 2 != 0)
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
                    obs.visualIndicator.transform.localScale = Vector3.one;
                    obs.visualIndicator.transform.localPosition = new Vector3(0, 0, distance / 10);
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
        public bool isDetected { get { return lastDistance < detectRange && lastDistance >= 0; } }
        public float detectRange = 20;
        public float lastDistance = -1;
    }
}
