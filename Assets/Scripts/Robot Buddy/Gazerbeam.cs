using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gazerbeam : MonoBehaviour
{
    public GazerbeamCatcher gazeFocus;
    public static Gazerbeam singleton;
    public Vector3 gazePoint;
    public static GazerbeamCatcher GazeFocus { get => singleton.gazeFocus; }
    public static Vector3 GazePoint { get => singleton.gazePoint; }

    private void Awake()
    {
        singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        float closestDistanceYet = -1;
        GazerbeamCatcher closestCatcherYet = null;
        Vector3 closestGazePointYet = Vector3.zero;

        foreach(RaycastHit hit in Physics.RaycastAll(transform.position, transform.forward))
        {
            if (hit.transform.GetComponent<GazerbeamCatcher>() != null)
            {
                if(hit.transform.parent.GetComponent<InterfaceNode>().inView)
                {
                    float frustumHeight = 2.0f * hit.distance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

                    float distance = Vector3.Distance(hit.point, hit.transform.position);
                    float adjustedDistance = distance / frustumHeight;

                    Debug.Log(adjustedDistance);

                    if (adjustedDistance < 0.15f)
                    {
                        if (closestDistanceYet == -1 || adjustedDistance < closestDistanceYet)
                        {
                            closestDistanceYet = adjustedDistance;
                            closestCatcherYet = hit.transform.GetComponent<GazerbeamCatcher>();
                            closestGazePointYet = hit.point;
                        }
                    }


                    Debug.DrawLine(transform.position, hit.point, Color.white);
                    Debug.DrawLine(hit.point, hit.transform.position, Color.cyan);
                }
            }
        }

        gazeFocus = closestCatcherYet;
        gazePoint = closestGazePointYet;
    }
}
