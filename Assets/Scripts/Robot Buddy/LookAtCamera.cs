using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Vector3 offset;
    public bool swivelRoll;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(offset);
        if (swivelRoll) transform.Rotate(new Vector3(0, 0, Camera.main.transform.localEulerAngles.z));
    }
}
