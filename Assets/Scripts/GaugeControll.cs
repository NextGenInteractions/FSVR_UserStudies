using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeControll : MonoBehaviour
{

    public GameObject needle;
    public float minAngle;
    public float maxAngle;

    public float minValue;
    public float maxValue;



    public void setValue(float value)
    {

        float angle = -Mathf.Clamp( Remap(value, minValue, maxValue, minAngle, maxAngle), minAngle, maxAngle);

        needle.transform.localRotation = Quaternion.Euler(angle, 0, 0); 

    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
