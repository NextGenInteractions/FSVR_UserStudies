using NextGen.VrManager.PivotManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StaticHandGrabbable : MonoBehaviour
{
    public bool isBeingGrabbed = false;
    public StaticHand.Handedness handedness;

    [SerializeField] public Transform leftHandIkTargets;
    [SerializeField] public Transform rightHandIkTargets;

    [Header("Hand Offsets")]
    [SerializeField] private Transform leftHandOffset;
    [SerializeField] private Transform rightHandOffset;

    [Header("Settings")]
    public bool sticksToHandAfterBeingGrabbed = false;

    private Rigidbody rb;

    private Pivot pivot;
    private Vector3 pivotStartingPos;
    private Quaternion pivotStartingRot;

    private StaticHand handGrabbingMe;

    // Start is called before the first frame update
    void Start()
    {
        pivot = GetComponentInChildren<Pivot>();
        pivotStartingPos = pivot.transform.localPosition;
        pivotStartingRot = pivot.transform.localRotation;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isBeingGrabbed)
        {
            BeGrabbed(handGrabbingMe);
        }
    }

    public void BeGrabbed(StaticHand staticHand)
    {
        handGrabbingMe = staticHand;

        pivot.transform.localPosition = staticHand.handedness == StaticHand.Handedness.Left ? leftHandOffset.localPosition : rightHandOffset.localPosition;
        pivot.transform.localRotation = staticHand.handedness == StaticHand.Handedness.Left ? leftHandOffset.localRotation : rightHandOffset.localRotation;
        isBeingGrabbed = true;

        rb.isKinematic = true;
    }

    public void BeDropped(Vector3 a, Vector3 b)
    {
        rb.isKinematic = false;

        Vector3 vector = b - a;
        vector *= 100;
        Debug.Log(vector);

        rb.AddForce(vector, ForceMode.VelocityChange);

        transform.position = pivot.transform.position;
        transform.rotation *= handGrabbingMe.handedness == StaticHand.Handedness.Left ? leftHandOffset.localRotation : rightHandOffset.localRotation;

        pivot.transform.localPosition = pivotStartingPos;
        pivot.transform.localRotation = pivotStartingRot;

        handGrabbingMe = null;

        isBeingGrabbed = false;


    }
}
