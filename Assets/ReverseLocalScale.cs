using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseLocalScale : MonoBehaviour
{
    public Transform toReverse;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1/toReverse.localScale.x, 1/toReverse.localScale.y, 1/toReverse.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
