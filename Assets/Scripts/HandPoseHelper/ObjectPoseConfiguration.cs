// ObjectPoseConfiguration handles the poses of hands when snapping to objects.  
// Allows poses to be rotated and slide based on the position of the tracked hands.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPoseConfiguration : MonoBehaviour {
    [Header("Cached Colliders")]
    public Collider colliderLeft;           // Collider on the left side respectively
    public Collider colliderRight;          // Collider on the right side respectively

    [Header("Hand Pose Meshes")]
    public GameObject leftHandPose;
    public GameObject rightHandPose;        //posed hand meshes that will be toggled out to replace the tracked hand meshes
    public HandPoseLerp handPoseLerpLeft;
    public HandPoseLerp handPoseLerpRight;
    public bool useLerp = false;


    // Use local axis gizmos to help in determining the proper vectors to use

    [Header("Rotational Freedom Settings")]
    public bool hasRotationFreedom;
    public Transform rotationRoot;          //Transform that will be rotated
    public Vector3 localRotationAxis;  //Axis of rotation, relative to this transform
    public Vector3 referenceVector;    //vector to compare the tracked vector with, relative to this transform
    public Vector3 trackedVector;      //vector relative to hand, compared with reference vector to determine angle to rotate "rotationRoot"
                                       //   ex: if tracked/reference vectors are alined, rotation should be zero.


    [Header("Linear Movement Settings")]
    public bool hasLinearFreedom;
    public Transform slideRoot;              //Transform that will be moved
    public Vector3 slideAxis;           //Axis of linear movement, relative to this transform
    public float minSlide, maxSlide;    //max/min distances, from starting point allong slide axis
    Vector3 slideStartPos;              //tracks initial position

    private GameObject currentTrackedHand;
    private PoseColliderController colliderController;

    /// <summary>
    /// Save the start position of the transform to move the hook later with tracked hand data
    /// </summary>
    private void Start()
    {
        if(slideRoot != null)
            slideStartPos = slideRoot.transform.position;
    }

    /// <summary>
    /// Reading grasp approach with LookAt function for calculating hand approach. And further moving the hand transform
    /// based on tracked hand transform in the constrained dimension
    /// </summary>
    private void Update()
    {
        if(currentTrackedHand != null)
        {
            if (hasRotationFreedom)
            {
                //Get vectors is world space
                Vector3 axis = transform.rotation * localRotationAxis;
                            Debug.DrawLine(transform.position, transform.position + axis); //debug
                Vector3 reference = transform.rotation * referenceVector;
                            //Debug.DrawLine(transform.position, transform.position + up);
                Vector3 tracked = currentTrackedHand.transform.rotation * trackedVector;
                            Debug.DrawLine(transform.position, transform.position + tracked);


                //find the direction rotated
                float dot = Vector3.Dot( Vector3.Cross(reference, tracked) , axis );
                float sign = Mathf.Sign(dot);

                //find angle around rotation axis
                float angle = Vector3.Angle(tracked, reference) * sign; 

                rotationRoot.transform.localRotation = Quaternion.AngleAxis(angle, localRotationAxis);
            }

            if (hasLinearFreedom)
            {
                //Get vectors is world space
                Vector3 target = currentTrackedHand.transform.position - slideStartPos;
                            Debug.DrawLine(transform.position, transform.position + target); //debug
                Vector3 axis = transform.rotation * slideAxis;
                            Debug.DrawLine(transform.position, transform.position + axis); 

                //get distance allong slide axis
                float distance = Vector3.Dot(target, axis);
                distance = Mathf.Clamp(distance, minSlide, maxSlide);

                //set slide position relative to the initial position of the slide root
                slideRoot.transform.position = axis * distance + slideStartPos;
            }

            if (!LeapPoseManager.getInstance().rightHandLeapMesh.activeSelf)
                LeapPoseManager.getInstance().unSnap(gameObject, true);
            if (!LeapPoseManager.getInstance().leftHandLeapMesh.activeSelf)
                LeapPoseManager.getInstance().unSnap(gameObject, false);

        }





    }
    /// <summary>
    /// We enable the hand posed variable on LeapPoseManager script when the live leapmotion hands enter object collider.
    /// </summary>
    /// <param name="handCollider"></param>
    private void OnTriggerEnter(Collider handCollider)
    {
        if (handCollider.gameObject.tag == "left_hand")
            ToggleHand(handCollider.gameObject, false, true, 1);
        else if (handCollider.gameObject.tag == "right_hand")
            ToggleHand(handCollider.gameObject, true, true, 1);
    }

    /// <summary>
    /// We enable the hand posed variable on LeapPoseManager script when the live leapmotion hands enter object collider.
    /// </summary>
    /// <param name="handCollider"></param>
    private void OnTriggerStay(Collider handCollider)
    {
        if (handCollider.gameObject.tag == "left_hand")
            ToggleHand(handCollider.gameObject, false, true);
        else if (handCollider.gameObject.tag == "right_hand")
            ToggleHand(handCollider.gameObject, true, true);
    }


    /// <summary>
    /// We disable the handPosed variable on LeapPoseManager script when the live leapmotion hands exit object collider.
    /// </summary>
    /// <param name="handCollider"></param>
    private void OnTriggerExit(Collider handCollider)
    {
        if (handCollider.gameObject.tag == "left_hand")
            ToggleHand(handCollider.gameObject, false, false, -1);
        else if (handCollider.gameObject.tag == "right_hand")
            ToggleHand(handCollider.gameObject, true, false, -1);
    }

    private void ToggleHand(GameObject hand, bool isRightHand, bool isActive, int state = 0)
    {
        if ((colliderController != null && colliderController.isColliding == isActive))
            return;
        if (colliderController != null)
        {
            if (colliderController.isColliding)
                colliderController.ColliderEvent(this, !isRightHand);
            colliderController.isColliding = isActive;
        }
        if (isActive)
        {
            if (useLerp && state == 1)
                GetHandPoseLerp(isRightHand).PlayforwardLerp();
            LeapPoseManager.getInstance().snap(transform.gameObject, isRightHand);
        }
        else
        {
            if (useLerp && state == -1)
                GetHandPoseLerp(isRightHand).PlaybackLerp();
            LeapPoseManager.getInstance().unSnap(transform.gameObject, isRightHand);
        }
        currentTrackedHand = isActive ? hand : null;
    }

    private HandPoseLerp GetHandPoseLerp(bool isRight)
    {
        return isRight ? handPoseLerpRight : handPoseLerpLeft;
    }

    /// <summary>
    /// Assign PoseColliderController that has control over this script
    /// Called by PoseColliderController
    /// </summary>
    /// <param name="controller"></param>
    public void SetColliderController (PoseColliderController controller)
    {
        colliderController = controller;
    }

    //Called by LeapPoseManager to cancel hand snapping
    public void forceRelease()
    {
        currentTrackedHand = null;
    }
}
