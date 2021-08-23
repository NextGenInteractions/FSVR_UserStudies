using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotCalibration : MonoBehaviour
{
    public Transform pivotRoot;
    public Vector3 relativePositionOfRoot, relativeAngleOfRoot;
    public KeyCode key = KeyCode.C;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go;
        if (pivotRoot == null && (go = GameObject.Find("Calibration - Root")) != null)
            pivotRoot = go.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
            Calibration();
    }

    void Calibration()
    {
        if (pivotRoot == null)
            return;
        transform.position = pivotRoot.position;
        transform.rotation = pivotRoot.rotation;
        if (relativeAngleOfRoot != Vector3.zero)
            transform.rotation *= Quaternion.Euler(relativeAngleOfRoot);
        if (relativePositionOfRoot != Vector3.zero)
            transform.position += pivotRoot.rotation * relativePositionOfRoot;
    }
}
