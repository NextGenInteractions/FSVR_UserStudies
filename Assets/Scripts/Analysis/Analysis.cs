using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Analysis : MonoBehaviour
{
    [HideInInspector]
    public float distance;
    [HideInInspector]
    public float mean;
    [HideInInspector]
    public float stdDev;
    int counter = 0;
    Dictionary<int, bool> testMap = new Dictionary<int, bool>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// Start Distance Analysis
    /// </summary>
    /// <param name="subject1"></param>
    /// <param name="subject2"></param>
    /// <param name="duration"></param>
    /// <param name="tickPerSec"></param>
    /// <param name="perFrame">optional, if true, regardless of duration and tickPerSec, it'll update per frame</param>
    /// <returns></returns>
    public int StartTwoObjectDistanceTest(GameObject subject1, GameObject subject2, float duration, float tickPerSec, bool perFrame)
    {
        bool twoObjTest = true;
        testMap.Add(++counter, twoObjTest);
        StartCoroutine(TwoObjectDistanceTest(subject1.transform, subject2.transform, duration, tickPerSec, perFrame, counter));
        return counter;
    }

    /// <summary>
    /// coroutine of ienumerator
    /// </summary>
    /// <param name="subject1"></param>
    /// <param name="subject2"></param>
    /// <param name="duration"></param>
    /// <param name="tickPerSec"></param>
    /// <param name="perFrame"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    IEnumerator TwoObjectDistanceTest(Transform subject1, Transform subject2, float duration, float tickPerSec, bool perFrame, int id)
    {
        // DataRecorder.instance.LogNote("Start Distance Analysis");

        // The rows of transform samples
        List<string> outputLines = new List<string>();

        List<double> _dis = new List<double>();

        int frameNumber = 0;

        float startTime = Time.time;
        float lastTime = Time.time;
        float totalTime = duration;


        /*
        // Format suggested by Regis
          Columns for tracking data file(csv)
          Frame#, timestamp, px1, py1, pz1, rx1, ry1, rz1, qx1, qy1, qz1, qw1. px2, py2, pz2, rx2, ry2, rz2, qx2, qy2, qz2, qw2, rpx2, rpy2, rpz2, rrx2, rry2, rrz2, rqx2, rqy2, rqz2, rqw2
          p = position reading
          r = rotation reading
          q = quaternion reading
          x, y, z = axis
          w = quaternion angle
          1 = reference device (such as vive puck)
          2 = test device (such as custom armband)(edited)
          r prefix = test device in relation to the first device local coordinate system
        */

        // Data transform header strings
        // Device 1 transform in world space
        string pos1X = "px1";
        string pos1Y = "py1";
        string pos1Z = "pz1";
        string rot1X = "rx1";
        string rot1Y = "ry1";
        string rot1Z = "rz1";
        string quat1X = "qx1";
        string quat1Y = "qy1";
        string quat1Z = "qz1";
        string quat1W = "qw1";

        // Device 2 transform in world space
        string pos2X = "px2";
        string pos2Y = "py2";
        string pos2Z = "pz2";
        string rot2X = "rx2";
        string rot2Y = "ry2";
        string rot2Z = "rz2";
        string quat2X = "qx2";
        string quat2Y = "qy2";
        string quat2Z = "qz2";
        string quat2W = "qw2";

        // Device 2 transforms relative to device 1 (i.e., in device 1 coordinate system)
        string rpos2X = "rpx2";
        string rpos2Y = "rpy2";
        string rpos2Z = "rpz2";
        string rrot2X = "rrx2";
        string rrot2Y = "rry2";
        string rrot2Z = "rrz2";
        string rquat2X = "rqx2";
        string rquat2Y = "rqy2";
        string rquat2Z = "rqz2";
        string rquat2W = "rqw2";


        // Add .csv header to list of rows
        string heading = string.Format("{0},{1}," +
                                        "{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}," +
                                        "{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}," +
                                        "{22},{23},{24},{25},{26},{27},{28},{29},{30},{31}",
                                        "frame#", "timestamp",
                                        pos1X, pos1Y, pos1Z, rot1X, rot1Y, rot1Z, quat1X, quat1Y, quat1Z, quat1W,
                                        pos2X, pos2Y, pos2Z, rot2X, rot2Y, rot2Z, quat2X, quat2Y, quat2Z, quat2W,
                                        rpos2X, rpos2Y, rpos2Z, rrot2X, rrot2Y, rrot2Z, rquat2X, rquat2Y, rquat2Z, rquat2W
                                      );
        outputLines.Add(heading);


        while (testMap[id])
        {
            if (perFrame)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSecondsRealtime(1f / tickPerSec);

            // Transform data
            Vector3 pos1 = subject1.position;
            Vector3 pos2 = subject2.position;
            Vector3 rot1 = subject1.eulerAngles;
            Vector3 rot2 = subject2.eulerAngles;
            Quaternion quat1 = subject1.rotation;
            Quaternion quat2 = subject2.rotation;

            // Compute device 2 transform relative to device 1 transform
            Vector3 rpos2 = subject1.InverseTransformPoint(pos2); // Transform device 2 World position to device 1's local space 
            Quaternion rquat2 = Quaternion.Inverse(quat1) * quat2; // Transform device 2 World Rotation to device 1's local space 
            Vector3 rrot2 = rquat2.eulerAngles;

            // Frame number and timestamp
            string outputLine = frameNumber.ToString() + ",";
            outputLine += (lastTime - startTime).ToString() + ",";

            // Device 1 transform data
            outputLine += string.Format("{0},{1},{2},", pos1.x.ToString("0.0000"), pos1.y.ToString("0.0000"), pos1.z.ToString("0.0000"));
            outputLine += string.Format("{0},{1},{2},", rot1.x.ToString("0.0000"), rot1.y.ToString("0.0000"), rot1.z.ToString("0.0000"));
            outputLine += string.Format("{0},{1},{2},{3},", quat1.x.ToString("0.0000"), quat1.y.ToString("0.0000"), quat1.z.ToString("0.0000"), quat1.w.ToString("0.0000"));

            // Device 2 transform data 
            outputLine += string.Format("{0},{1},{2},", pos2.x.ToString("0.0000"), pos2.y.ToString("0.0000"), pos2.z.ToString("0.0000"));
            outputLine += string.Format("{0},{1},{2},", rot2.x.ToString("0.0000"), rot2.y.ToString("0.0000"), rot2.z.ToString("0.0000"));
            outputLine += string.Format("{0},{1},{2},{3},", quat2.x.ToString("0.0000"), quat2.y.ToString("0.0000"), quat2.z.ToString("0.0000"), quat2.w.ToString("0.0000"));

            // Device 2 transform data relative to device 1
            outputLine += string.Format("{0},{1},{2},", rpos2.x.ToString("0.0000"), rpos2.y.ToString("0.0000"), rpos2.z.ToString("0.0000"));
            outputLine += string.Format("{0},{1},{2},", rrot2.x.ToString("0.0000"), rrot2.y.ToString("0.0000"), rrot2.z.ToString("0.0000"));
            outputLine += string.Format("{0},{1},{2},{3}", rquat2.x.ToString("0.0000"), rquat2.y.ToString("0.0000"), rquat2.z.ToString("0.0000"), rquat2.w.ToString("0.0000"));



            // Add csv string row of data to list of rows
            outputLines.Add(outputLine);

            distance = Vector3.Distance(pos1, pos2);
            _dis.Add(distance);
            totalTime -= (Time.time - lastTime);
            lastTime = Time.time;
            if (totalTime <= 0)
                testMap[id] = false;


            frameNumber++;

        }

        mean = (float) (_dis.Sum() / _dis.Count());
        stdDev = (float) StdDev(_dis, false);

        DataRecorder.instance.LogNote("Starting Distance," + _dis[0].ToString("0.00000"));
        DataRecorder.instance.LogNote("Average Distance," + mean.ToString("0.00000"));
        DataRecorder.instance.LogNote("Standard Deviation," + stdDev.ToString("0.00000"));

        // DataRecorder.instance.LogNote("End of Distance Analysis");

        // The following is not optimal as LogNote opens and closes the file every time it is called.
        // A better way to do this would be to open once, write all data, and then close the file
        foreach (string line in outputLines)
        {
            DataRecorder.instance.LogNote(line);
        }

    }

        /// <summary>
        /// End two object ditance test by id
        /// </summary>
        /// <param name="testID"></param>
        public void EndTwoObjectDistanceTest(int testID)
    {
        EndTestByID(testID);
    }

    /// <summary>
    /// General function to end test by id
    /// </summary>
    /// <param name="testID"></param>
    public void EndTestByID(int testID)
    {
        if (testMap.ContainsKey(testID))
            testMap[testID] = false;
    }


    /// <summary>
    /// Start three object distance test
    /// equivelant to two object distance test in round robin way
    /// </summary>
    /// <param name="subject1"></param>
    /// <param name="subject2"></param>
    /// <param name="subject3"></param>
    /// <param name="duration"></param>
    /// <param name="tickPerSec"></param>
    /// <param name="perFrame">optional, if true, regardless of duration and tickPerSec, it'll update per frame</param>
    /// <returns></returns>
    public int StartThreeObjectDistanceTest(GameObject subject1, GameObject subject2, GameObject subject3, float duration, float tickPerSec, bool perFrame)
    {
        bool threeObjTest = true;
        testMap.Add(++counter, threeObjTest);
        StartCoroutine(ThreeObjectDistanceTest(subject1.transform, subject2.transform, subject3.transform, duration, tickPerSec, perFrame, counter));
        return counter;
    }

    /// <summary>
    /// ienumerator coroutine for three object
    /// </summary>
    /// <param name="subject1"></param>
    /// <param name="subject2"></param>
    /// <param name="subject3"></param>
    /// <param name="duration"></param>
    /// <param name="tickPerSec"></param>
    /// <param name="perFrame"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    IEnumerator ThreeObjectDistanceTest(Transform subject1, Transform subject2, Transform subject3, float duration, float tickPerSec, bool perFrame, int id)
    {
        DataRecorder.instance.LogNote("Start Three Object Distance Test");
        string pos1X = "subject1 posX";
        string pos1Y = "subject1 posY";
        string pos1Z = "subject1 posZ";
        string pos2X = "subject2 posX";
        string pos2Y = "subject2 posY";
        string pos2Z = "subject2 posZ";
        string pos3X = "subject3 posX";
        string pos3Y = "subject3 posY";
        string pos3Z = "subject3 posZ";
        List<double> _dis12 = new List<double>();
        List<double> _dis13 = new List<double>();
        List<double> _dis23 = new List<double>();
        float totalTime = duration;
        float lastTime = Time.time;
        while (testMap[id])
        {
            if (perFrame)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSecondsRealtime(1f / tickPerSec);
            Vector3 pos1 = subject1.position;
            Vector3 pos2 = subject2.position;
            Vector3 pos3 = subject3.position;
            pos1X += "," + pos1.x.ToString("0.0000");
            pos1Y += "," + pos1.y.ToString("0.0000");
            pos1Z += "," + pos1.z.ToString("0.0000");
            pos2X += "," + pos2.x.ToString("0.0000");
            pos2Y += "," + pos2.y.ToString("0.0000");
            pos2Z += "," + pos2.z.ToString("0.0000");
            pos3X += "," + pos3.x.ToString("0.0000");
            pos3Y += "," + pos3.y.ToString("0.0000");
            pos3Z += "," + pos3.z.ToString("0.0000");
            float dis12 = Vector3.Distance(pos1, pos2);
            float dis13 = Vector3.Distance(pos1, pos3);
            float dis23 = Vector3.Distance(pos3, pos2);
            _dis12.Add(dis12);
            _dis13.Add(dis13);
            _dis23.Add(dis23);
            totalTime -= (Time.time - lastTime);
            lastTime = Time.time;
            if (totalTime <= 0)
                testMap[id] = false;
        }
        double mean12 = _dis12.Sum() / _dis12.Count();
        double stdDev12 = StdDev(_dis12, false);
        double mean13 = _dis13.Sum() / _dis13.Count();
        double stdDev13 = StdDev(_dis13, false);
        double mean23 = _dis23.Sum() / _dis23.Count();
        double stdDev23 = StdDev(_dis23, false);
        DataRecorder.instance.LogNote(pos1X);
        DataRecorder.instance.LogNote(pos1Y);
        DataRecorder.instance.LogNote(pos1Z);
        DataRecorder.instance.LogNote(pos2X);
        DataRecorder.instance.LogNote(pos2Y);
        DataRecorder.instance.LogNote(pos2Z);
        DataRecorder.instance.LogNote(pos3X);
        DataRecorder.instance.LogNote(pos3Y);
        DataRecorder.instance.LogNote(pos3Z);
        DataRecorder.instance.LogNote("Starting Distance - Subject 1 & 2," + _dis12[0].ToString("0.0000"));
        DataRecorder.instance.LogNote("Average Distance - Subject 1 & 2," + mean12.ToString("0.0000"));
        DataRecorder.instance.LogNote("Standard Deviation - Subject 1 & 2," + stdDev12.ToString("0.0000"));
        DataRecorder.instance.LogNote("Starting Distance - Subject 1 & 3," + _dis13[0].ToString("0.0000"));
        DataRecorder.instance.LogNote("Average Distance - Subject 1 & 3," + mean13.ToString("0.0000"));
        DataRecorder.instance.LogNote("Standard Deviation - Subject 1 & 3," + stdDev13.ToString("0.0000"));
        DataRecorder.instance.LogNote("Starting Distance - Subject 2 & 3," + _dis23[0].ToString("0.0000"));
        DataRecorder.instance.LogNote("Average Distance - Subject 2 & 3," + mean23.ToString("0.0000"));
        DataRecorder.instance.LogNote("Standard Deviation - Subject 2 & 3," + stdDev23.ToString("0.0000"));
//        DataRecorder.instance.LogNote("End Three Object Distance Test");
    }

    /// <summary>
    /// end three object distance test by id
    /// </summary>
    /// <param name="testID"></param>
    public void EndThreeObjectDistanceTest(int testID)
    {
        EndTestByID(testID);
    }

    // Return the standard deviation of an array of Doubles.
    //
    // Doubles are used since squaring the small differences could be a problem since squaring small numbers make them even smaller. 
    //
    // If the second argument is True, evaluate as a sample.
    // If the second argument is False, evaluate as a population.
    public static double StdDev(List<double> values,
        bool as_sample)
    {
        // Get the mean.
        double mean = values.Sum() / values.Count();

        // Get the sum of the squares of the differences
        // between the values and the mean.
        var squares_query =
            from double value in values
            select (value - mean) * (value - mean); // Squaring small values results in even smaller values adding risk of additional roundoff error. Hence use doubles.
        double sum_of_squares = squares_query.Sum();

        if (as_sample)
        {
            return System.Math.Sqrt(sum_of_squares / (values.Count() - 1));
        }
        else
        {
            return System.Math.Sqrt(sum_of_squares / values.Count());
        }
    }
}
