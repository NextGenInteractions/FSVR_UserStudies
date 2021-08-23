using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTransform : MonoBehaviour
{
    public Transform toMatch;

    // Update is called once per frame
    void Update()
    {
        if(toMatch)
        {
            //transform.position = toMatch.position;
            if(!toMatch.GetComponent<SteamVRExtensionTrackingData>().isLostTracking)
                transform.eulerAngles = new Vector3(0, toMatch.eulerAngles.y, 0);
        }
    }
}
