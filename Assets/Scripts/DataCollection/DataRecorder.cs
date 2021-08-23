using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataRecorder : MonoBehaviour
{
    public static DataRecorder instance;
    public int attemptNum = 0;
    private string testID = "";
    private string dir;
    private string curDir;
    public string timestamp;
    private Dictionary<int, TransformPathList> pathMap = new Dictionary<int, TransformPathList>();

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        dir = Application.persistentDataPath + "/DataCollection";
        curDir = Application.persistentDataPath + "/DataCollection";
#elif UNITY_ANDROID
    dir = Application.persistentDataPath + "/DataCollection";
    curDir = Application.persistentDataPath + "/DataCollection";
#elif UNITY_IPHONE
    dir = Application.persistentDataPath + "/DataCollection";
    curDir = Application.persistentDataPath + "/DataCollection";
#else
    dir = Application.dataPath + "/DataCollection";
    curDir = Application.dataPath + "/DataCollection";
#endif
        instance = this;
        CreateDir();
        if (GetLastTestID().Equals("none"))
            SetTestID("0");
        else
            SetTestID(GetLastTestID());
        if (GetLastAttemptNumber() == 0)
            SetAttemptNum(1);
        else
            SetAttemptNum(GetLastAttemptNumber());
    }

    // Update is called once per frame
    void Update()
    {
        string time = System.DateTime.UtcNow.ToLocalTime().ToString("MM-dd-yyyy HH:mm:ss");
        timestamp = time;
    }

    private void CreateDir()
    {
        if (!Directory.Exists(curDir))
        {
            Directory.CreateDirectory(curDir);
        }
    }

    private void Log(string line, bool isPath = false)
    {
        string id = GetLastTestID();
        int num = GetLastAttemptNumber();
        string type = (isPath) ? "path" : "log";
        StreamWriter nextGenSW = new StreamWriter(curDir + "/" + Application.productName + "-" + Application.version + "_" + type + "_" + id + "_" + num + ".csv", true);
        nextGenSW.WriteLine(line);
        nextGenSW.Close();
    }

    private void LogJson(string line)
    {
        string id = GetLastTestID();
        int num = GetLastAttemptNumber();
        StreamWriter nextGenSW = new StreamWriter(curDir + "/" + Application.productName + "-" + Application.version + "_path_" + id + "_" + num + ".json", true);
        nextGenSW.WriteLine(line);
        nextGenSW.Close();
    }

    public static double GetTimestamp()
    {
        return System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    public void SetAttemptNum(int index)
    {
        attemptNum = index;
        PlayerPrefs.SetInt("attemptNum", index);
    }

    public void SetTestID (string testid)
    {
        testID = testid;
        PlayerPrefs.SetString("testID", testid);
        curDir = dir + "/" + testid;
        CreateDir();
    }

    public string GetLastTestID ()
    {
        if (PlayerPrefs.HasKey("testID"))
            return PlayerPrefs.GetString("testID");
        else
            return "none";
    }

    public int GetLastAttemptNumber()
    {
        if (PlayerPrefs.HasKey("attemptNum"))
            return PlayerPrefs.GetInt("attemptNum");
        else
            return 0;
    }

    public void LogNote(string note)
    {
        LogRaw(note);
    }

    public void LogRaw(string line, bool isPath = false)
    {
        Log(timestamp + "," + line, isPath);
    }

    public void LogTimer(string note, string timer)
    {
        LogRaw(note + ", " + timer);
    }

    public void LogTransform(int id, string note, Transform trans)
    {
        if (!pathMap.ContainsKey(id))
        {
            pathMap.Add(id, new TransformPathList());
            pathMap[id].name = trans.name;
        }
        pathMap[id].note = note;
        pathMap[id].positionList.Add(trans.position);
        pathMap[id].rotationList.Add(trans.eulerAngles);
        LogRaw("Log Transform " + name + ": " + note + ", " + trans.position.x.ToString("0.000") + ", " + trans.position.y.ToString("0.000") + ", " + trans.position.z.ToString("0.000") + ", " + trans.eulerAngles.x.ToString("0.000") + ", " + trans.eulerAngles.y.ToString("0.000") + ", " + trans.eulerAngles.z.ToString("0.000"), true);
    }

    public void EndTransformLog(int id)
    {
        LogJson(JsonUtility.ToJson(pathMap[id]));
    }

    [System.Serializable]
    class TransformPathList
    {
        public string name;
        public string note;
        public List<Vector3> positionList = new List<Vector3>();
        public List<Vector3> rotationList = new List<Vector3>();
    }

}
