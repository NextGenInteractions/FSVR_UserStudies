using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArduTouchEventHandler : MonoBehaviour
{
    [Header("Settings")]
    public float doubleClickThreshold = 0.5f;

    [Header("Event Triggers")]
    public UnityEvent onAllClicks;
    public UnityEvent onSingleClick;
    public UnityEvent onDoubleClick;
    public UnityEvent onHover;

    private float timeSinceLastClick = 0.5f;

    void Update()
    {
        timeSinceLastClick += Time.deltaTime;
    }

    public void Click()
    {
        onAllClicks.Invoke();

        if(timeSinceLastClick < doubleClickThreshold)
        {
            onDoubleClick.Invoke();
        }
        else
        {
            onSingleClick.Invoke();
        }

        timeSinceLastClick = 0;
    }

    public void Hover()
    {
        onHover.Invoke();
    }

    public void SingleDebug()
    {
        Debug.Log(gameObject.name + " just single clicked!");
    }

    public void DoubleDebug()
    {
        Debug.Log(gameObject.name + " just double clicked!");
    }
}
