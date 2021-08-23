using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataFilterManager : MonoBehaviour
{
    public static DataFilterManager trackingInstance;
    public enum Type
    {
        TrackingData,
        Ultrasound
    }
    public Type type = Type.TrackingData;
    public int filterCap = 30;
    public float motionThreshold = 0.1f;
    [Tooltip("Tolerance of outliners: if it reaches the limit, outliners'll become normal positional dataset")]
    public int tolerance = 30;
    [Tooltip("Exceptional Vector3: if the new positional data is contained as an outliner, it won't be considered for calculation")]
    public List<Vector3> exceptionalVector = new List<Vector3>() { Vector3.zero };
    public List<float> exceptionalValue = new List<float>() { 0 };
    public bool isOn = true;
    public bool isDetectedOutline { get; private set; } = false;
    Dictionary<SteamVRExtensionTrackingData, FilterInfo> poseList = new Dictionary<SteamVRExtensionTrackingData, FilterInfo>();
    Dictionary<SteamVRDeviceData, FilterInfo> devicePosList = new Dictionary<SteamVRDeviceData, FilterInfo>();

    // Start is called before the first frame update
    void Awake()
    {
        if (type == Type.TrackingData)
            trackingInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsDetectOutliner(SteamVRExtensionTrackingData pose)
    {
        if (!poseList.ContainsKey(pose))
            return false;
        return poseList[pose].vectorOutliers.Count > 0;
    }

    public int TolerancePoint(SteamVRExtensionTrackingData pose)
    {
        if (!poseList.ContainsKey(pose))
            return 0;
        return poseList[pose].vectorOutliers.Count;
    }

    public Vector3 FilterPositionPose(SteamVRExtensionTrackingData pose, Vector3 newPos)
    {
        if (poseList.ContainsKey(pose))
        {
            FilterInfo filterInfo = poseList[pose];
            if (!(isDetectedOutline = filterInfo.DetectOutlierV3(newPos, 3)))
            {
                filterInfo.AddPos(newPos);
                filterInfo.vectorOutliers.Clear();
            }
            else if (!exceptionalVector.Contains(newPos))
                filterInfo.AddVectorOutlier(newPos);
        }
        else
        {
            FilterInfo filterInfo = new FilterInfo(this);
            poseList.Add(pose, filterInfo);
            filterInfo.AddPos(newPos);
        }
        if (isOn)
            return poseList[pose].GetLastPos();
        else
            return newPos;
    }

    public Vector3 FilterPositionDeviceIndex(SteamVRDeviceData pose, Vector3 newPos)
    {
        if (devicePosList.ContainsKey(pose))
        {
            FilterInfo filterInfo = devicePosList[pose];
            if (!(isDetectedOutline = filterInfo.DetectOutlierV3(newPos, 3)))
            {
                filterInfo.AddPos(newPos);
                filterInfo.vectorOutliers.Clear();
            }
            else if (!exceptionalVector.Contains(newPos))
                filterInfo.AddVectorOutlier(newPos);
        }
        else
        {
            FilterInfo filterInfo = new FilterInfo(this);
            devicePosList.Add(pose, filterInfo);
            filterInfo.AddPos(newPos);
        }
        if (isOn)
            return devicePosList[pose].GetLastPos();
        else
            return newPos;
    }


    /// <summary>
    /// 0 - 1x std means 34.1%
    /// 1x - 2x std means 13.6%
    /// 2x - 3x std means 2.1%
    /// >3x means 0.1% outlier
    /// </summary>
    /// <param name="values"></param>
    /// <returns>the standard deviation of an array of Doubles.</returns>
    public static float StdDev(List<float> values)
    {
        // Get the mean.
        float mean = values.Sum() / values.Count();

        // Get the sum of the squares of the differences
        // between the values and the mean.
        var squares_query =
            from float value in values
            select (value - mean) * (value - mean);
        float sum_of_squares = squares_query.Sum();

        return Mathf.Sqrt(sum_of_squares / values.Count());
    }
    [System.Serializable]
    public class FilterInfo
    {
        public List<float> xList = new List<float>();
        public List<float> yList = new List<float>();
        public List<float> zList = new List<float>();
        public List<Vector3> vectorOutliers = new List<Vector3>();
        public List<float> valueOutliners = new List<float>();
        public List<float> valueList = new List<float>();
        public Vector3 v3STD = Vector3.zero;
        public float vSTD = 0;
        private DataFilterManager dfm;

        public FilterInfo(DataFilterManager dataFilterManager)
        {
            dfm = dataFilterManager;
        }

        public Vector3 GetLastPos()
        {
            if (xList.Count > 0)
            {
                int last = xList.Count - 1;
                return new Vector3(xList[last], yList[last], zList[last]);
            }
            else
                return Vector3.zero;
        }

        public float GetLastValue()
        {
            if (valueList.Count > 0)
            {
                int last = xList.Count - 1;
                return valueList[last];
            }
            else
                return 0;
        }

        public void AddPos(Vector3 newPos)
        {
            if (xList.Count >= dfm.filterCap)
            {
                xList.RemoveAt(0);
                yList.RemoveAt(0);
                zList.RemoveAt(0);
            }
            xList.Add(newPos.x);
            yList.Add(newPos.y);
            zList.Add(newPos.z);
            v3STD = new Vector3(StdDev(xList), StdDev(yList), StdDev(zList));
        }

        public void AddVectorOutlier (Vector3 newPos)
        {
            vectorOutliers.Add(newPos);
            if (vectorOutliers.Count >= dfm.tolerance)
            {
                Clear();
                foreach (Vector3 v in vectorOutliers)
                    AddPos(v);
                vectorOutliers.Clear();
            }
        }

        public void AddValueOutliner(float newValue)
        {
            valueOutliners.Add(newValue);
            if (valueOutliners.Count >= dfm.tolerance)
            {
                Clear();
                foreach (float v in valueOutliners)
                    AddValue(v);
                valueOutliners.Clear();
            }
        }

        public void AddValue(float newValue)
        {
            if (valueList.Count >= dfm.filterCap)
            {
                valueList.RemoveAt(0);
            }
            valueList.Add(newValue);
            vSTD = StdDev(valueList);
        }

        public void Clear()
        {
            xList.Clear();
            yList.Clear();
            zList.Clear();
            valueList.Clear();
            v3STD = Vector3.zero;
            vSTD = 0;
        }

        public bool DetectOutlierV3(Vector3 newPos, int levelStd)
        {
            return xList.Count >= dfm.filterCap && Vector3.Distance(GetLastPos(), newPos) > dfm.motionThreshold && (v3STD.x * levelStd < newPos.x || v3STD.y * levelStd < newPos.y || v3STD.z * levelStd < newPos.z);
        }
        public bool DetectOutlinerValue(float newValue, int levelStd)
        {
            return valueList.Count >= dfm.filterCap && vSTD * levelStd < newValue && newValue > dfm.motionThreshold;
        }

    }
}
