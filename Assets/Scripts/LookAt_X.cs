using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt_X : MonoBehaviour
{
    public GameObject ObjectofDesire;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = ObjectofDesire.transform.position;
        targetPosition.y = transform.position.y;
        Quaternion target = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 2f);
    }
}
