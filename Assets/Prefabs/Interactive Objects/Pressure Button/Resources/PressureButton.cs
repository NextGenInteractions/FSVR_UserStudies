using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public bool enableGizmos = false;

    //Component references.
    private AudioSource aud;

    //Component references on other GameObjects.
    public Transform buttonSpring;
    public Transform buttonFace;

    //Values which can be changed in the inspector.
    public float castOriginOffset;
    public float maxUp;
    public float maxPressDistance;

    //Watch variables.
    public float travelPercentage;
    private float actuationPercentage = 1;
    public float resetPercentage = 0.5f;
    private bool readyToActuate = true;
    void Awake()
    {
        aud = GetComponent<AudioSource>();
    }

    private void OnDrawGizmos()
    {
        if (enableGizmos)
        {
            Gizmos.matrix = Matrix4x4.identity;

            float max = maxUp + castOriginOffset;
            float min = castOriginOffset + (maxUp - maxPressDistance);

            Vector3 castOrigin = transform.position - (transform.up * castOriginOffset);
            Vector3 springHeight = Vector3.zero;
            Vector3 zeroHeight = Vector3.zero;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(castOrigin, .01f);
            Gizmos.DrawWireSphere(transform.position, .01f);

            if (!Physics.BoxCast(castOrigin, buttonFace.lossyScale / 2, transform.up, out RaycastHit hit, transform.rotation, max))
            {
                Gizmos.color = Color.green;
                springHeight = transform.up * max;
                zeroHeight = Vector3.up * max;
                travelPercentage = 0;
            }
            else if (hit.distance > min)
            {
                Gizmos.color = Color.yellow;
                springHeight = transform.up * hit.distance;
                zeroHeight = Vector3.up * hit.distance;
                travelPercentage = (max - hit.distance) / (max - min);
            }
            else
            {
                Gizmos.color = Color.red;
                springHeight = transform.up * min;
                zeroHeight = Vector3.up * min;
                travelPercentage = 1;
            }

            Gizmos.DrawRay(castOrigin, springHeight);
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawWireCube((Vector3.down * castOriginOffset) + zeroHeight, buttonFace.lossyScale);

            buttonSpring.position = castOrigin + springHeight;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float max = maxUp + castOriginOffset;
        float min = castOriginOffset + (maxUp - maxPressDistance);

        Vector3 castOrigin = transform.position - (transform.up * castOriginOffset);
        Vector3 springHeight = Vector3.zero;

        if (!Physics.BoxCast(castOrigin, buttonFace.lossyScale / 2, transform.up, out RaycastHit hit, transform.rotation, max))
        {
            Gizmos.color = Color.green;
            springHeight = transform.up * max;
            travelPercentage = 0;
        }
        else if (hit.distance > min)
        {
            Gizmos.color = Color.yellow;
            springHeight = transform.up * hit.distance;
            travelPercentage = (max - hit.distance) / (max - min);
        }
        else
        {
            Gizmos.color = Color.red;
            springHeight = transform.up * min;
            travelPercentage = 1;
        }

        buttonSpring.position = castOrigin + springHeight;

        if (travelPercentage >= actuationPercentage && readyToActuate) Actuate();
        if (travelPercentage <= resetPercentage && !readyToActuate) Reset();
    }

    void Actuate()
    {
        aud.Play();
        readyToActuate = false;
    }

    private void Reset()
    {
        readyToActuate = true;
    }
}
