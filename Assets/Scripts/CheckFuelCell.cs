using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFuelCell : MonoBehaviour {

    public string inductorTubeTagged;
    public bool succesfulPlacement = false;
    public GameObject fuelCell;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered");
        if(other.gameObject.tag.Equals(inductorTubeTagged))
        {
            Debug.Log("Correct thing is now in");
            succesfulPlacement = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Something exited");

        if (other.gameObject.tag.Equals(inductorTubeTagged))
        {
            Debug.Log("Correct thing is now out");
            succesfulPlacement = false;
        }
    }
}
