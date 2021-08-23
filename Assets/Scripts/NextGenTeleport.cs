using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class NextGenTeleport : MonoBehaviour
{
    public SteamVR_Action_Boolean teleportAction;
    public Transform teleHand;
    public Material rayMat;
    public LayerMask layerMask;
    LineRenderer line;
    bool onRay = false;
    SteamVRExtensionTrackingData hand;
    // Start is called before the first frame update
    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.material = rayMat;
        line.startWidth = 0.05f;
        line.endWidth = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        if (hand == null)
            hand = teleHand.GetComponent<SteamVRExtensionTrackingData>();
        Vector3 hitPos = Vector3.zero;
        if (onRay)
        {
            RaycastHit hit;
            Ray ray = new Ray(teleHand.position, teleHand.forward);
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                line.positionCount = 2;
                hitPos = hit.point;
                line.SetPosition(0, teleHand.position);
                line.SetPosition(1, hitPos);
            }
        }
        else
            line.positionCount = 0;
        if (teleportAction.GetStateDown(hand.inputSource))
            onRay = true;
        if (teleportAction.GetStateUp(hand.inputSource))
        {
            onRay = false;
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                Vector3 offset = transform.position - cam.transform.position;
                offset.y = 0;
                Vector3 endPos = hitPos + offset;
                StartCoroutine(FastMove(endPos));
            }
        }
    }

    IEnumerator FastMove(Vector3 dest)
    {
        Vector3 start = transform.position;
        float timer = 0.5f;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
            transform.position = Vector3.Lerp(start, dest, 0.5f - timer);
        }
    }
}
