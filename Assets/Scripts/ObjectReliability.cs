using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReliability : MonoBehaviour
{
    public enum ReliableType
    {
        Position,
        Rotation,
        Both
    }

    [Header("General Setting")]
    public bool autoStart = true; // check if start the motion on start of this script
    public Transform root; // root object that this object follows
    public ReliableType reliableType = ReliableType.Both; // reliability type: Position or/and Rotation
    public Motion rotationalMotion; // rotational motion specific setting and information
    public Motion positionalMotion; // positional motion specific setting and information
    [Range(0, 1.0f)]
    public float smoothLerpThreshold = 0.0f; // threshold for a proportion of the motion that does smooth lerp

    public bool constrainY = false;  // Set to true to only rotate about the y axis and not move position up/down
    public bool setYToZeroInstead = false;

    private float disThreshold = 0.1f; // threshold for triggering smooth lerp based on distance of the motion
    // restored information for a period of motion
    private Vector3 position, localPos;
    private Quaternion rotation, localRot;
    private float posDis, rotDis;

    // Start is called before the first frame update
    void Start()
    {
        if (autoStart)
            Invoke("StartMotion", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        // null check
        if (root == null)
            return;
        // Positional motion
        if (reliableType != ReliableType.Rotation && positionalMotion.isStart)
        {
            positionalMotion.currentTime += Time.deltaTime;
            float lerpValue = positionalMotion.currentTime / positionalMotion.delay;
            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            // a way to keep the motion with steady speed
            // lerpValue *= (positionalMotion.speed / Vector3.Distance(transform.position, pos));

            /* Smooth Lerp Calculation
             * Motion: make projected curve motion smoothly to motion paht of next read of root potion
             * Calculation: calculate normalized cyclical motion given last proportion of current motion and projection of next motion
             */ 
            if (positionalMotion.currentTime >= positionalMotion.delay * smoothLerpThreshold && posDis > disThreshold)
            {
                Quaternion relativeRot = Quaternion.Lerp(Quaternion.LookRotation(position - transform.position), Quaternion.LookRotation(root.position - transform.position), Time.deltaTime / positionalMotion.delay);
                pos = ((relativeRot * (position - transform.position).normalized).normalized * posDis) + transform.position;
                lerpValue = Time.deltaTime / positionalMotion.delay;
            }
            lerpValue *= positionalMotion.speed; // Speed applied

            // Use linear lerp or spherical lerp

            // Assume Linear motion if rotationalMotion does not exist
            if (positionalMotion == null)
            {
                pos = Vector3.Lerp(transform.position, position, lerpValue);
            }
            else
            {
                pos = positionalMotion.lerpType == Motion.LerpType.Linear ?
                    Vector3.Lerp(transform.position, position, lerpValue) :
                    Vector3.Slerp(transform.position, position, lerpValue);
            }

            if (float.IsNaN(pos.x))
                pos = new Vector3(0.0f, 0.0f, 0.0f);

            // apply result
            transform.position = pos;

            if (setYToZeroInstead)
                transform.position = new Vector3(pos.x, 0, pos.z);
        }

        // Rotational motion
        if (reliableType != ReliableType.Position && rotationalMotion.isStart)
        {
            rotationalMotion.currentTime += Time.deltaTime;
            float lerpValue = rotationalMotion.currentTime / rotationalMotion.delay;

            // Speed applied
            lerpValue *= rotationalMotion.speed;

            // Use linear lerp or spherical lerp
            Quaternion rot;

            // Assume Linear motion if rotationalMotion does not exist
            if (rotationalMotion == null)
            {
                rot = Quaternion.Lerp(transform.rotation, rotation, lerpValue);
            }
            else
            {
                rot = rotationalMotion.lerpType == Motion.LerpType.Linear ?
                    Quaternion.Lerp(transform.rotation, rotation, lerpValue) :
                    Quaternion.Slerp(transform.rotation, rotation, lerpValue);
            }

            // apply result
            transform.rotation = rot;
        }
    }

    /// <summary>
    /// Start Motion as public method
    /// Invoke and restore motion routine(s)
    /// </summary>
    public void StartMotion ()
    {
        if (reliableType != ReliableType.Rotation && positionalMotion.routine == null)
        {
            ProjectedPosition();
            positionalMotion.currentTime = 0;
            positionalMotion.routine = StartCoroutine(StartPositionalMotionRoutine());
        }
        if (reliableType != ReliableType.Position && rotationalMotion.routine == null)
        {
            rotationalMotion.currentTime = 0;
            rotationalMotion.routine = StartCoroutine(StartRotationalMotionRoutine());
        }
    }

    /// <summary>
    /// Stop Motion as public method
    /// Terminate and clear motion routine(s)
    /// </summary>
    public void StopMotion ()
    {
        positionalMotion.isStart = false;
        rotationalMotion.isStart = false;
        if (positionalMotion.routine != null)
            StopCoroutine(StartPositionalMotionRoutine());
        if (rotationalMotion.routine != null)
            StopCoroutine(StartRotationalMotionRoutine());
    }


    /// <summary>
    /// Start Positional Motion Routine
    /// and call method to calculation projected position for motion
    /// </summary>
    /// <returns></returns>
    IEnumerator StartPositionalMotionRoutine ()
    {
        positionalMotion.isStart = true;
        while (positionalMotion.isStart)
        {
            yield return new WaitForSeconds(positionalMotion.delay);
            ProjectedPosition();
            positionalMotion.currentTime = 0;
        }

    }


    /// <summary>
    /// Start Rotational Motion Routine
    /// and call method to calculation projected rotation for motion
    /// </summary>
    /// <returns></returns>
    IEnumerator StartRotationalMotionRoutine()
    {
        rotationalMotion.isStart = true;
        while (rotationalMotion.isStart)
        {
            yield return new WaitForSeconds(rotationalMotion.delay);
            ProjectedRotation();
            rotationalMotion.currentTime = 0;
        }

    }

    /// <summary>
    /// Calculate projected position
    /// apply local offset and world offset
    /// restore informtion
    /// </summary>
    void ProjectedPosition()
    {
        Vector3 pos = root.position + root.TransformVector(positionalMotion.localOffset);

        // This doesn't appear to be necessary for our needs. Keeping it here in case it is necessary for other needs.
        // if (constrainY)
        // pos.y = 0.0f;

        pos += positionalMotion.worldOffset;
        Vector3 projectedPos = (pos - transform.position) * 1.0f;
        position = transform.position + projectedPos;
        posDis = Vector3.Distance(transform.position, position);
    }

    /// <summary>
    /// Calculate projected rotation
    /// apply local offset and world offset
    /// restore informtion
    /// </summary>
    void ProjectedRotation ()
    {
        Quaternion rot;

        if (constrainY)
        {
            // Constrain to only rotate about the y axis
            Quaternion constrainedRotation;
            constrainedRotation = Quaternion.identity;  // Not including results in an error when trying to set constrainedRotation.eulerAngles
            Vector3 eulerAngles = root.rotation.eulerAngles;
            float yRotation = eulerAngles.y;

/*
            //
            // Calculate quaternion rotation around the y up axis
            //

            // Get the right vector of the root rotation
            Vector3 transformed = root.rotation * Vector3.right;

            // Project transformed right vector onto xz plane
            Vector3 flattened = transformed - (Vector3.Dot(transformed, Vector3.up) * Vector3.up);
            flattened.Normalize();

            // Get angle between original vector and projected transform to get angle around y up axis
            float a = Mathf.Rad2Deg * Mathf.Acos((Vector3.Dot(Vector3.right, flattened)));

            // calculate crossproduct vector is up or down and reverse direction if down
            if (Vector3.Cross(Vector3.right, flattened).y < 0)
                a = -a;

            //if(Mathf.Asin(Vector3.Dot(Vector3.right, flattened)) < 0.0f)
            //a = -a;

            //if (flattened.z < 0)
            //a = -a;
*/

            // Set all angles other than Y to zero
            eulerAngles.x = 0.0f;
            eulerAngles.y = yRotation;
            eulerAngles.z = 0.0f;
            constrainedRotation.eulerAngles = eulerAngles;

            rot = constrainedRotation * Quaternion.Euler(root.TransformDirection(rotationalMotion.localOffset));
        }

        else // Do not constrain to Y rotation
        {
            rot = root.rotation * Quaternion.Euler(root.TransformDirection(rotationalMotion.localOffset));
        }

        rot *= Quaternion.Euler(rotationalMotion.worldOffset);
        rotation = rot;
    }

    /// <summary>
    /// Internal Class: Motion
    /// Restore motion specific information and variables
    /// so they will not confuse with others 
    /// </summary>
    [System.Serializable]
    public class Motion
    {
        public enum LerpType
        {
            Linear,
            Curve
        }

        [Header("Motion")]
        public float delay = 0.0f; // the delay to update motion info
        // the speed of motion range from 0.1 to 2
        // where 1 being the normal --- it finishes the motion at the exact moment of next read
        // less than 1 the motion will not be finished til next read
        // greater than 1 the motion will finish before next read
        [Range(0.1f, 2.0f)]
        public float speed = 1f;
        public LerpType lerpType = LerpType.Linear; // lerp type
        [HideInInspector]
        public float currentTime = 0; // time helps track of timeelapse
        [HideInInspector]
        public Coroutine routine; // store the routine in case of termination at running
        [HideInInspector]
        public bool isStart; // flag to start

        // offset information in Vector3
        [Header("Offset")]
        public Vector3 localOffset; 
        public Vector3 worldOffset;

    }
}
