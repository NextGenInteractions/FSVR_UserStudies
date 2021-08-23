using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduTouchScrollRect : MonoBehaviour, IArduTouchElement
{
    public float scaleFactor = 1.5f;

    private TouchpadToCanvas touchpad;
    private Vector3 lastCursorLocation;

    public Transform content;

    public float framesSinceLastHover;

    // Start is called before the first frame update
    void Awake()
    {
        Transform check = transform;
        while (check.GetComponent<TouchpadToCanvas>() == null) check = check.parent;
        touchpad = check.GetComponent<TouchpadToCanvas>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        framesSinceLastHover++;
    }

    public void OnArduTouchTap()
    {

    }

    public void OnArduTouchHover()
    {
        Debug.Log("Hovering!");
        if (framesSinceLastHover < 2)
        {
            Debug.Log(Vector3.Distance(lastCursorLocation, touchpad.LocalTweenedCursorLocation));
            content.localPosition -= (lastCursorLocation - touchpad.LocalTweenedCursorLocation) * scaleFactor;
        }
        lastCursorLocation = touchpad.LocalTweenedCursorLocation;
        framesSinceLastHover = 0;
    }
}
