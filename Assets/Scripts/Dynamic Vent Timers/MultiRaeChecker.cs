using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRaeChecker : MonoBehaviour
{
    [SerializeField] private float checkRadius = 0.125f;
    [SerializeField] private bool lastMultiRaePresent = false;
    [SerializeField] private bool multiRaePresent = false;

    // Update is called once per frame
    void Update()
    {
        multiRaePresent = false;

        foreach (Collider collider in Physics.OverlapSphere(transform.position, checkRadius))
        {
            if(collider.GetComponent<IsAMultiRae>() != null)
            {
                multiRaePresent = true;
                break;
            }
        }

        if(!lastMultiRaePresent && multiRaePresent)
            MultiRaeEnter();

        if (lastMultiRaePresent && !multiRaePresent)
            MultiRaeExit();

        lastMultiRaePresent = multiRaePresent;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

    private void OnDrawGizmosSelected()
    {
        
    }

    private void MultiRaeEnter()
    {
        GetComponent<DynamicTimer>().StartTimer();
    }

    private void MultiRaeExit()
    {
        GetComponent<DynamicTimer>().StopTimer();
    }
}
