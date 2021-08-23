using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDependentAdjustment : MonoBehaviour
{
    public enum Relativity
    {
        World,
        Local
    }

    // Axis that is used to calcualate to determine if the root is in a segment
    public enum AxisEffectiveness
    {
        Axis_X,
        Axis_Y,
        Axis_Z
    }

    [Header("General Setting")]
    public List<DependentAdjustmentSegment> segments = new List<DependentAdjustmentSegment>(); // list of segment that holds motion information

    [Header("Root Setting")]
    public Transform root; // root gameobject transform
    public Relativity dependencyOfRoot = Relativity.Local; // the dependency of whole calculation
    public AxisEffectiveness effectiveAxis = AxisEffectiveness.Axis_Y; // Axis that is used to calcualate to determine if the root is in a segment

    [Header("Dependentee Setting")]
    public Relativity motionRelativity = Relativity.Local; // the relativity of this gameobject's motion
    public float speed = 1; // movement speed

    private float boundCheckInAngleThreshold = 30; // angle that checks if the root motion is within segment setup
    private float boundCheckInPercent = 0.1f; // the biased percentage of distance betweet lower and upper bound
    private int currentIndex = 0; // current index of the segment the root is in

    // Start is called before the first frame update
    void Start()
    {
        // initialization of each segments
        segments.ForEach(segment => segment.Initialization(boundCheckInPercent));
    }

    // Update is called once per frame
    void Update()
    {
        // edge case and null check
        if (segments.Count == 0 && root == null)
        {
            Debug.LogWarning(gameObject.name + ": has no segment in list or root object is not assigned!");
            return;
        }
        
        // get effective axis value from the root position
        Vector3 rootPos = (dependencyOfRoot == Relativity.Local) ? root.localPosition : root.position;
        rootPos = (effectiveAxis == AxisEffectiveness.Axis_X) ? new Vector3(rootPos.x, 0, 0) : (effectiveAxis == AxisEffectiveness.Axis_Y) ? new Vector3(0, rootPos.y, 0) : new Vector3(0, 0, rootPos.z);

        // check if the root is in current segment
        // default current index is 0
        if (segments[currentIndex].CheckInBound(rootPos, boundCheckInAngleThreshold))
        {
            Vector3 updatedPos = segments[currentIndex].UpdatedPosition();
            if (motionRelativity == Relativity.World)
                transform.position = Vector3.LerpUnclamped(transform.position, updatedPos, Time.deltaTime * speed);
            else
                transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, updatedPos, Time.deltaTime * speed);
        }
        else
        {
            // search for the segment that is in bound with root
            for (int i = 0; i < segments.Count; ++i)
            {
                if (segments[i].CheckInBound(rootPos, boundCheckInAngleThreshold))
                {
                    currentIndex = i;
                    break;
                }
            }
        }
    }
}

/// <summary>
/// Segment that holds information and behavier
/// based on bounds and calcualtion
/// </summary>
[System.Serializable]
public class DependentAdjustmentSegment
{
    public Vector3 rootLowerBound; // lower bound of root motion on active axis
    public Vector3 rootUpperBound; // upper bound of root motion on active axis
    public Vector3 dependenteeLowerPos; // respectively the position of dependentee when the root is at the position of lower bound
    public Vector3 dependenteeUpperrPos; // respectively the position of dependentee when the root is at the position of upper bound

    private Vector3 rootBoundVector; // normalized the vector based on lower and upper bound of root
    private float distance; // distance between lower and upper bound
    private float distanceThreshold = 1; // the threshold of bound distance, affected by boundCheckInPercent from ObjectDependentAdjustment script
    private float lerpValue = 0; // relative range 0 - 1 based on root position interval within the bounds 

    /// <summary>
    /// Initialization called by ObjectDependentAdjustment
    /// </summary>
    /// <param name="percent"></param>
    public void Initialization(float percent)
    {
        CalcBoundVector();
        CalcDistanceThreshold(percent);
    }

    /// <summary>
    /// Check if the root is within the bound
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public bool CheckInBound (Vector3 pos, float threshold)
    {
        // calculate the similarity between the dependentee vector and root bound vector
        Vector3 dependenteeVector = (rootUpperBound - pos).normalized;
        float angle = Vector3.Angle(rootBoundVector, dependenteeVector);

        // calcualte and compare the distances to determine if the root is within the bounds
        float lowerDis = Vector3.Distance(rootLowerBound, pos);
        float upperDis = Vector3.Distance(rootUpperBound, pos);
        float scaleDis = lowerDis + upperDis;
        lerpValue = lowerDis / scaleDis;

        return (angle <= threshold) ? scaleDis <= distanceThreshold : false;
    }

    /// <summary>
    /// Update dependentee's position
    /// </summary>
    /// <returns> (Vector3) position </returns>
    public Vector3 UpdatedPosition ()
    {
        return Vector3.LerpUnclamped(dependenteeLowerPos, dependenteeUpperrPos, lerpValue);
    }

    /// <summary>
    /// calcualte bound vector
    /// called by Initialization()
    /// </summary>
    private void CalcBoundVector()
    {
        rootBoundVector = (rootUpperBound - rootLowerBound).normalized;
    }

    /// <summary>
    /// calculate distance and the threshold
    /// called by Initialization()
    /// </summary>
    /// <param name="percent">boundCheckInPercent from ObjectDependentAdjustment script</param>
    private void CalcDistanceThreshold (float percent)
    {
        distance = Vector3.Distance(rootLowerBound, rootUpperBound);
        distanceThreshold = distance * (percent + 1);
    }
}