using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyApplyIfAngleChanged : MonoBehaviour
{
    public Transform toMatch;
    public float degreeThreshold = 5;

    private SteamVRExtensionTrackingData tracking;

    // Start is called before the first frame update
    void Start()
    {
        //RefreshAngle();

        tracking = toMatch.GetComponent<SteamVRExtensionTrackingData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tracking.isLostTracking) return;

        transform.position = toMatch.position;

        if (Quaternion.Angle(transform.rotation, toMatch.rotation) > degreeThreshold)
            RefreshAngle();
    }

    void RefreshAngle()
    {

        transform.rotation = toMatch.rotation;
    }
}
