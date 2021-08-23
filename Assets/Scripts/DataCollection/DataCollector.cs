using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
    public static DataCollector instance;
    int id = 0;
    readonly Dictionary<int, MapInfo> map = new Dictionary<int, MapInfo>();

    /// <summary>
    /// Test Variables
    /// </summary>
    int test1 = -1, test2 = -1;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        ///Test Code
        if (Input.GetKeyDown(KeyCode.A))
        {
            //test1 = RegisterCounter("test 1");
            test1 = RegisterTimer("test 3");
            test2 = RegisterTimer("test 4");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //test2 = RegisterCounter("test 2");
            PauseLog(test1);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            IncrementCounter(test1, "test 1 counting");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            IncrementCounter(test2, "test 2 counting");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EndLog(test1);
            EndLog(test2);
        }
    }

    /// <summary>
    /// Register a transform routine
    /// </summary>
    /// <param name="note"></param>
    /// <param name="trans"></param>
    /// <param name="tickPerSec"></param>
    /// <returns>routine instance id</returns>
    public int RegisterTransform(string note, Transform trans, float tickPerSec)
    {
        IdRegistration(++id);
        Coroutine coroutine = StartCoroutine(TransformLog(id, note, trans, tickPerSec));
        Register(coroutine);
        return id;
    }

    /// <summary>
    /// Register a logging routine
    /// </summary>
    /// <param name="note"></param>
    /// <returns>routine instance id</returns>
    public int RegisterLog(string note)
    {
        IdRegistration(++id);
        Coroutine coroutine = StartCoroutine(Logging(id, note));
        Register(coroutine);
        return id;
    }

    /// <summary>
    /// Register a timer routine
    /// </summary>
    /// <param name="note"></param>
    /// <returns>routine instance id</returns>
    public int RegisterTimer(string note)
    {
        IdRegistration(++id);
        Coroutine coroutine = StartCoroutine(Timer(id, note));
        Register(coroutine);
        return id;
    }

    /// <summary>
    /// Register a counter routine
    /// </summary>
    /// <param name="note"></param>
    /// <returns>routine instance id</returns>
    public int RegisterCounter(string note)
    {
        IdRegistration(++id);
        Coroutine coroutine = StartCoroutine(Counting(id, note));
        Register(coroutine);
        return id;
    }

    /// <summary>
    /// Get logging routine information
    /// note: for instance on Timer, it will return the timespan on timer
    /// </summary>
    /// <param name="idIndex"></param>
    /// <returns></returns>
    public object GetRegisterorInfo(int idIndex)
    {
        if (map.ContainsKey(idIndex))
        {
            return map[idIndex].info;
        }
        else
            return 0f;
    }

    public void IncrementCounter(int idIndex, string note)
    {
        if(map.ContainsKey(idIndex))
        {
            map[idIndex].info =(int)map[idIndex].info + 1;
            DataRecorder.instance.LogNote("Counter #" + idIndex + " Incrementing: " + note);
        }
    }

    /// <summary>
    /// Pause the routine that is registered for logging
    /// </summary>
    /// <param name="idIndex"></param>
    public void PauseLog(int idIndex)
    {
        if (map.ContainsKey(idIndex))
        {
            map[idIndex].pause = true;
        }
    }

    /// <summary>
    /// resume the paused routine
    /// </summary>
    /// <param name="idIndex"></param>
    public void ResumeLog(int idIndex)
    {
        if (map.ContainsKey(idIndex))
        {
            map[idIndex].pause = false;
        }
    }

    private void Register(Coroutine coroutine)
    {
        if (map.ContainsKey(id))
        {
            if (map[id].routine != null)
                StopCoroutine(map[id].routine);
            map[id].routine = coroutine;
        }
        else
        {
            map.Add(id, new MapInfo() { 
                routine = coroutine
            });
        }
    }

    private void IdRegistration(int idIndex)
    {
        if (map.ContainsKey(idIndex))
        {
            if (map[idIndex].routine != null)
                StopCoroutine(map[idIndex].routine);
            map[idIndex] = new MapInfo();
        }
        else
        {
            map.Add(idIndex, new MapInfo());
        }
    }

    /// <summary>
    /// End Logging routine natually
    /// </summary>
    /// <param name="idIndex"></param>
    public void EndLog (int idIndex)
    {
        if (map.ContainsKey(idIndex) && map[idIndex].routine != null)
        {
            map[idIndex].checker = true;
        }
    }

    /// <summary>
    /// Unregister any logging routine
    /// Note: this will stop the routine without logging out
    /// </summary>
    /// <param name="idIndex"></param>
    public void Unregister(int idIndex)
    {
        if (map.ContainsKey(idIndex))
        {
            StopCoroutine(map[idIndex].routine);
            RemoveMapping(idIndex);
        }
    }

    IEnumerator Counting(int idIndex, string note)
    {
        object counter = 0;
        map[idIndex].checker = false;
        map[idIndex].info = 0;
        DataRecorder.instance.LogNote("Start Counter #" + idIndex + " : " + note);
        while (!map[idIndex].checker)
        {
            yield return null;
        }
        DataRecorder.instance.LogTimer("End Counter #" + idIndex + " : " + note, map[idIndex].info.ToString());
        RemoveMapping(idIndex);
    }

    IEnumerator Logging (int idIndex, string note)
    {
        yield return new WaitForEndOfFrame();
        DataRecorder.instance.LogNote(note);
        RemoveMapping(idIndex);
    }
    IEnumerator WaitForBool(int idIndex)
    {
        yield return null;
    }
    IEnumerator TransformLog(int idIndex, string note, Transform trans, float tickPerSec)
    {
        if (tickPerSec <= 0)
            tickPerSec = 60;
        map[idIndex].checker = false;
        DataRecorder.instance.LogNote("Start Log Path #" + idIndex + " : " + note);
        while (!map[idIndex].checker)
        {
            if (!map[idIndex].pause)
                DataRecorder.instance.LogTransform(idIndex, note, trans);
            yield return new WaitForSecondsRealtime(1 / tickPerSec);
        }
        DataRecorder.instance.EndTransformLog(idIndex);
        DataRecorder.instance.LogNote("End Log Path #" + idIndex + " : " + note);
        RemoveMapping(idIndex);
    }
    IEnumerator Timer(int idIndex, string note)
    {
        map[idIndex].checker = false;
        map[idIndex].info = 0f;
        System.DateTime startTime = System.DateTime.Now;
        DataRecorder.instance.LogNote("Start Timer #" + idIndex + " : " + note);
        while (!map[idIndex].checker)
        {
            if (!map[idIndex].pause)
            {
                System.TimeSpan diffP = (map[idIndex].pauseInfo == null) ? new System.TimeSpan() : (System.TimeSpan)map[idIndex].pauseInfo;
                map[idIndex].info = System.DateTime.Now - startTime - diffP;
            }
            else
            {
                System.TimeSpan diffI = (map[idIndex].info == null) ? new System.TimeSpan() : (System.TimeSpan)map[idIndex].info;
                map[idIndex].pauseInfo = System.DateTime.Now - startTime - diffI;
            }
            yield return null;
        }
        System.TimeSpan diff = (map[idIndex].pauseInfo == null) ? new System.TimeSpan() : (System.TimeSpan)map[idIndex].pauseInfo;
        System.TimeSpan deltatime = System.DateTime.Now - startTime - diff;
        DataRecorder.instance.LogTimer("End Timer #" + idIndex + " : " + note, deltatime.TotalSeconds.ToString());
        RemoveMapping(idIndex);
    }

    private void RemoveMapping(int idIndex)
    {
        if (map.ContainsKey(idIndex))
        {
            if (map[idIndex].routine != null)
                StopCoroutine(map[idIndex].routine);
            map.Remove(idIndex);
        }
    }

    class MapInfo
    {
        public Coroutine routine;
        public object info;
        public bool checker;
        public bool pause;
        public object pauseInfo;
    }
}