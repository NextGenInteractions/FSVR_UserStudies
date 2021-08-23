using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AveragedMotion : MonoBehaviour {

    public int arrayLength = 10;
    public Transform tracker;
    private Vector3[] motionDataArray;
    private Quaternion[] rotationDataArray;
    private int frameNumber = 0;

    // Use this for initialization
    void Start () {
        motionDataArray = new Vector3[arrayLength];
        rotationDataArray = new Quaternion[arrayLength];
    }
	
	// Update is called once per frame
	void Update () {

        frameNumber++;
        if(frameNumber > arrayLength)
        {
            GetComponent<Valve.VR.SteamVR_TrackedObject>().enabled = false;
            this.enabled = false;
            return;
        }

        shiftArray();

        motionDataArray[arrayLength - 1] = tracker.position;
        rotationDataArray[arrayLength - 1] = tracker.rotation;
        AverageOut();
    }

    void AverageOut()
    {
        Vector3 averagePos = new Vector3();
        Quaternion averageRot = new Quaternion();

        for (int i = 1; i < arrayLength; i++)
        {
            averagePos += motionDataArray[i];
            averageRot = Quaternion.Slerp(averageRot, rotationDataArray[i], 1 / i);

        }
        transform.position = averagePos / arrayLength;
        transform.rotation = averageRot;
    }

    void shiftArray()
    {
        for (int i = 0; i < arrayLength - 1; i++)
        {
            motionDataArray[i] = motionDataArray[i + 1];
            rotationDataArray[i] = rotationDataArray[i + 1];
        }
    }
}
