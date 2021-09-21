using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTimer : MonoBehaviour
{
    [SerializeField] private string timerName;
    private string TimerName { get { return $"Dynamic Timer: {timerName}"; } }

    public bool hasStarted = false;

    // Update is called once per frame
    void Update()
    {
        if(!hasStarted && MetricManager.isLive)
        {
            MetricManager.CreateTimer(TimerName, false);
            hasStarted = true;
        }

        if(hasStarted && !MetricManager.isLive)
        {
            hasStarted = false;
        }
    }

    public void StartTimer()
    {
        MetricManager.StartTimer(TimerName);
        MetricManager.LogTimerTime(TimerName);
    }

    public void StopTimer()
    {
        MetricManager.StopTimer(TimerName);
    }
}
