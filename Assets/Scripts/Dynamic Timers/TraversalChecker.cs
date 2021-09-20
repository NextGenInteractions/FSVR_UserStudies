using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversalChecker : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> allDestinations = new List<BoxCollider>();
    [SerializeField] private bool traversing;
    private bool lastTraversing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(MetricManager.isLive)
        {
            traversing = true;

            foreach (BoxCollider destination in allDestinations)
            {
                foreach (Collider col in Physics.OverlapBox(
                    destination.transform.position + (destination.center * destination.transform.lossyScale.x),
                    new Vector3(
                        (destination.size.x * destination.transform.lossyScale.x) / 2,
                        (destination.size.y * destination.transform.lossyScale.y) / 2,
                        (destination.size.z * destination.transform.lossyScale.z) / 2
                    )))
                {
                    if (col.tag == "PlayerTag")
                        traversing = false;
                }
            }

            if (!lastTraversing && traversing)
                StartTimer();

            if (lastTraversing && !traversing)
                StopTimer();

            lastTraversing = traversing;
        }
    }

    void StartTimer()
    {
        GetComponent<DynamicTimer>().StartTimer();
    }

    void StopTimer()
    {
        GetComponent<DynamicTimer>().StopTimer();
    }
}
