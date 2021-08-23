using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchpadInterpreter : SerialInterpreter
{
    public float untouchDetectionInterval = 0.05f;

    [SerializeField] private float lastSerialX;
    [SerializeField] private float lastSerialY;
    [SerializeField] private float timeSinceLastTouch;

    public bool Touching { get { return (untouchDetectionInterval > timeSinceLastTouch && gameObject.activeInHierarchy); } }

    public Vector2 rawPoint;
    public Vector2 lastRawPoint;
    public Vector2 tweenPoint;

    public float timeSpentOnCurrentRaw;
    public float timeSinceLastTap = 1;
    public float timeSinceLastDoubleTap = 1;

    public float tweenPercentage = 0.05f;
    public float doubleTapThreshold = 0.5f;

    public Transform cursor;

    [Header("Settings")]
    public bool flipX;
    public bool flipY;

    //Events
    public delegate void Action(Vector2 coordinates);
    public static event Action OnTap;

    public float LeapDistance { get { return Vector2.Distance(rawPoint, lastRawPoint); } }

    private void Awake()
    {
        baudRate = 19200;
    }

    public override void ReceiveLine(string line)
    {
        //Debug.Log(string.Format("{0} has received serial message: {1}", name, line));

        string[] split = line.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
        float num = -1;
        switch (split[0].ToLower())
        {
            case "y":
                /* Note that X and Y value from Arduino is actually reversed in Unity
                 * Keep in mind if there's any update in the future
                 */
                num = -1;
                if (float.TryParse(split[1], out num) && num > -1)
                {
                    int raw = (int)num;
                    raw--;
                    if (flipX)
                        raw = 12 - raw;
                    raw++;

                    lastSerialX = raw;
                }
                break;

            case "x":
                num = -1;
                if (float.TryParse(split[1], out num) && num > -1)
                {
                    int raw = (int)num;
                    raw--;
                    if (flipY)
                        raw = 8 - raw;
                    raw++;
                    lastSerialY = raw;
                    timeSinceLastTouch = 0;
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastTouch += Time.deltaTime;
        timeSinceLastTap += Time.deltaTime;
        timeSinceLastDoubleTap += Time.deltaTime;

        if (Touching) timeSpentOnCurrentRaw += Time.deltaTime;
        else timeSpentOnCurrentRaw = 0;

        if (Touching && tweenPoint != rawPoint)
            TweenTowards(rawPoint);
    }

    private void LateUpdate()
    {
        Ping();
    }

    private void Ping()
    {
        if (Touching) //If touching...
        {
            Vector2 point = new Vector2(lastSerialX, lastSerialY);
            if (rawPoint != point) //If the raw point has moved...
            {
                if (rawPoint != Vector2.zero) //If touching before (not a "tap")...
                    lastRawPoint = rawPoint;
                else
                {
                    tweenPoint = point;
                    if (timeSinceLastTap < doubleTapThreshold)
                    {
                        if (timeSinceLastDoubleTap < doubleTapThreshold)
                        {
                            //Triple tap.
                        }
                        else
                        {
                            //Double tap.
                        }

                        timeSinceLastDoubleTap = 0;
                    }
                    else
                    {
                        //Single tap.
                    }
                    timeSinceLastTap = 0;
                    OnTap?.Invoke(point);
                }
                rawPoint = point;

                timeSpentOnCurrentRaw = 0;
            }
        }
        else //If not touching...
        {
            rawPoint = Vector2.zero;
            lastRawPoint = Vector2.zero;
            tweenPoint = Vector2.zero;
        }
    }

    public void TweenTowards(Vector2 _point)
    {
        if (rawPoint == null) return;
        if (tweenPoint == null) { tweenPoint = rawPoint; return; }
        Vector2 difference = rawPoint - tweenPoint;
        tweenPoint += (difference * tweenPercentage); 
    }
}
