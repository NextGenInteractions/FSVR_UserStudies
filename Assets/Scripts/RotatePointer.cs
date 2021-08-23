using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePointer : MonoBehaviour {

    [Range(-0.1f, 1)] public float rotationScale;
    private float targetRotation = 0.0f;
    [Range(0, 1)] public float speed = 0.1f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        targetRotation = Remap(rotationScale, -0.1f, 1.0f, -27.0f, 250.0f);
        RotateAnObjectOnPivot(targetRotation);

    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void RotateAnObjectOnPivot(float value)
    {
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation = Quaternion.Euler(0, targetRotation, 0);
        transform.localRotation = Quaternion.Lerp(fromRotation,toRotation,Time.deltaTime * speed);
    }

}
