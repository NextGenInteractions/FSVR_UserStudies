using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{

    public float speed = 1;
    public Vector3 axis = new Vector3(0, 1, 0);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axis, Time.deltaTime * speed);
    }
}
