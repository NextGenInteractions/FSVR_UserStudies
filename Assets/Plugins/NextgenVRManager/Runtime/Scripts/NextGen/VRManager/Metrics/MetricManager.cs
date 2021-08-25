using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class MetricManager : MonoBehaviour
{
    public static string sessionName;
    public static bool isLive = false;
    public static float sessionDuration = 0;
    public static float lapDuration;

    private static StreamWriter writer;

    public static Dictionary<string, Tally> tallies = new Dictionary<string, Tally>();
    public static Dictionary<string, Timer> timers = new Dictionary<string, Timer>();

    public static Action<Tally> onTallyCreated;
    public static Action<Timer> onTimerCreated;
    private void OnDisable()
    {
        if (isLive)
            SessionInterrupted();
    }

    void Update()
    {
        if (isLive)
        {
            sessionDuration += Time.deltaTime;
            lapDuration += Time.deltaTime;

            foreach(Timer timer in timers.Values)
            {
                if(timer.active)
                    timer.time += Time.deltaTime;
            }
        }
    }

    public static void StartLive(string _sessionName)
    {
        if(!isLive)
        {
            sessionName = _sessionName;
            isLive = true;
            sessionDuration = 0;
            lapDuration = 0;

            writer = new StreamWriter($"{Application.dataPath}/SessionLogs/{DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss")}_{sessionName}.csv");
            writer.AutoFlush = true;

            LogEvent($"Session '{sessionName}' started.");
        }

    }

    public static void StopLive()
    {
        if(isLive)
        {
            LogEvent("Session ended.");

            WriteLine("Final Values:");
            foreach (Tally tally in tallies.Values)
                WriteLine($"Tally '{tally.name}': {tally.count}");
            foreach (Timer timer in timers.Values)
                WriteLine($"Timer '{timer.name}': {TimeToMSD(timer.time)}");

            isLive = false;
            writer.Close();
        }
    }

    public static void SessionInterrupted()
    {
        LogEvent("Session interrupted -- ending session...");
        StopLive();
    }

    public static void ToggleIsLive(string _sessionName = "untitled")
    {
        if (!isLive)
            StartLive(_sessionName);
        else
            StopLive();
    }

    public static void NewLap()
    {
        LogEvent($"New lap started (last lap duration: {TimeToMSD(lapDuration)}).");

        lapDuration = 0;
    }

    public static string TimeToMSD(float timestamp)
    {
        int minutes = 0;
        int seconds = 0;

        while (timestamp > 60)
        {
            minutes++;
            timestamp -= 60;
        }
        while (timestamp > 1)
        {
            seconds++;
            timestamp--;
        }

        int remainder = Mathf.FloorToInt(timestamp * 10);

        return $"{minutes}:{seconds:00}.{remainder}";
    }

    public static void LogEvent(string body, string type = "System")
    {
        LogEvent(sessionDuration, type, body);
    }


    public static void LogEvent(float timestamp, string type, string body)
    {
        writer.WriteLine($"{DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss")},{TimeToMSD(timestamp)},{type},{body}");
    }

    public static void WriteLine(string line)
    {
        writer.WriteLine(line);
    }

    public static void CreateTally(string _tallyName, int _startingCount)
    {
        if (!tallies.ContainsKey(_tallyName))
        {
            tallies.Add(_tallyName, new Tally(_tallyName, _startingCount));
            onTallyCreated.Invoke(tallies[_tallyName]);
        }
        else
            Debug.LogWarning($"A tally already exists with the name '{_tallyName}'.");
    }

    public static void IncrementTally(string _tallyName, string _optionalNote = "")
    {
        if (tallies.ContainsKey(_tallyName))
        {
            tallies[_tallyName].count++;
            LogEvent($"Tally '{_tallyName}' has been incremented to a value of {tallies[_tallyName].count}.","Tally");
            if (_optionalNote != "")
                LogEvent(_optionalNote, "Tally");
        }
        else
            Debug.LogWarning($"No tally with the name '{_tallyName}' exists to be incremented.");
    }

    public static void DecrementTally(string _tallyName, string _optionalNote = "")
    {
        if (tallies.ContainsKey(_tallyName))
        {
            tallies[_tallyName].count--;
            LogEvent($"Tally '{_tallyName}' has been decremented to a value of {tallies[_tallyName].count}.", "Tally");
            if (_optionalNote != "")
                LogEvent(_optionalNote, "Tally");
        }
        else
            Debug.LogWarning($"No tally with the name '{_tallyName}' exists to be decremented.");
    }

    public static void CreateTimer(string _timerName, bool _startCountingImmediately = true)
    {
        if (!timers.ContainsKey(_timerName))
        {
            timers.Add(_timerName, new Timer(_timerName, _startCountingImmediately));
            onTimerCreated.Invoke(timers[_timerName]);
            string andStarted = _startCountingImmediately ? " and started" : "";
            LogEvent($"Timer created{andStarted}: {_timerName}");
        }
        else
            Debug.LogWarning($"A timer already exists with the name '{_timerName}'.");
    }

    public static void LogTimerTime(string _timerName)
    {
        if (timers.ContainsKey(_timerName))
            LogEvent($"Timer '{_timerName}' logged with time of {TimeToMSD(timers[_timerName].time)}.","Timer");
        else
            Debug.LogWarning($"No timer exists with the name {_timerName} to be logged.");
    }

    public static void StopTimer(string _timerName)
    {
        if (timers.ContainsKey(_timerName))
        {
            if (timers[_timerName].active)
            {
                LogEvent($"Timer '{_timerName}' stopped with time of {TimeToMSD(timers[_timerName].time)}.", "Timer");
                timers[_timerName].active = false;
            }
            else
                Debug.LogWarning($"Can't stop timer with the name {_timerName} because that timer is already stopped!");
        }
        else
            Debug.LogWarning($"No timer exists with the name {_timerName} to be stopped.");
    }

    public static void StartTimer(string _timerName)
    {
        if (timers.ContainsKey(_timerName))
        {
            if (!timers[_timerName].active)
                timers[_timerName].active = true;
            else
                Debug.LogWarning($"Can't start timer with the name {_timerName} because that timer is already active!");
        }
        else
            CreateTimer(_timerName, true);
    }

    public class Event
    {
        public float timestamp;
        public string type;
        public string body;
    }

    public class Tally
    {
        public string name;
        public int count;

        public Tally(string _name, int _startingCount)
        {
            name = _name;
            count = _startingCount;
        }
    }

    public class Timer
    {
        public string name;
        public float time;
        public bool active;

        public Timer(string _name, bool _startCountingImmediately)
        {
            name = _name;
            time = 0;
            active = _startCountingImmediately;
        }
    }
}
