using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToSpawnPoint : MonoBehaviour
{
    [SerializeField] private float yThreshold = 1;

    private Vector3 startPos;
    private Quaternion startRot;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < yThreshold)
        {
            transform.position = startPos;
            transform.rotation = startRot;

            Rigidbody rb = GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
}
