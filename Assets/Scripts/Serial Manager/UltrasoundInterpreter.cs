using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class UltrasoundInterpreter : SerialInterpreter
{
    [SerializeField] public float rawDistance;
    [SerializeField] public float avgDistance;
    private List<float> lastDistances = new List<float>();

    [Header("Threshold")]
    public float threshold = 0;
    public bool rawLessThanThreshold = false;
    public bool avgLessThanThreshold = false;

    private void Awake()
    {
        baudRate = 9600;
    }

    public override void ReceiveLine(string line)
    {
        string toParse = line.Substring(line.IndexOf(':') + 1);
        rawDistance = float.Parse(toParse);

        lastDistances.Add(rawDistance);
        if (lastDistances.Count > 25) lastDistances.RemoveAt(0);

        float sum = 0;
        foreach (float dist in lastDistances) sum += dist;
        avgDistance = sum / lastDistances.Count;
    }

    private void Update()
    {
        rawLessThanThreshold = rawDistance < threshold;
        avgLessThanThreshold = avgDistance < threshold;
    }
}
