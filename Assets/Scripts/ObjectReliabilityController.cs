using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReliabilityController : MonoBehaviour
{

    [Header("General Setting")]
    public ObjectReliability.ReliableType reliableType = ObjectReliability.ReliableType.Both; // reliability type: Position or/and Rotation
    public ObjectReliability.Motion rotationalMotion; // rotational motion specific setting and information
    public ObjectReliability.Motion positionalMotion; // positional motion specific setting and information
    [Range(0, 1.0f)]
    public float smoothLerpThreshold = 0.1f; // threshold for a proportion of the motion that does smooth lerp

    public List<GameObject> followerList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in followerList)
        {
            ObjectReliability or = go.AddComponent<ObjectReliability>();
            or.root = this.transform;
            or.reliableType = reliableType;
            or.rotationalMotion = rotationalMotion;
            or.positionalMotion = positionalMotion;
            or.smoothLerpThreshold = smoothLerpThreshold;
            or.StartMotion();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
