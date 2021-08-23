using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ArduTouchButton : MonoBehaviour, IArduTouchElement
{
    [Header("Hover Animations")]
    private Color idleColor = Color.white;
    private Color hoverColor = Color.gray;
    private Color clickColor = Color.black;

    //State descriptors.
    private bool pointerHover;
    private float timeSinceLastArduTouchHover = 0.1f;
    private bool ArduTouchHover { get { return timeSinceLastArduTouchHover < 0.1f; } }
    private bool Hover { get { return pointerHover || ArduTouchHover; } }

    private float timeSinceLastClick = 0.1f;
    private bool JustClicked { get { return timeSinceLastClick < 0.1f; } }

    //Component references.
    private Image img;
    private ArduTouchEventHandler eventHandler;

    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
        eventHandler = GetComponent<ArduTouchEventHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastArduTouchHover += Time.deltaTime;
        timeSinceLastClick += Time.deltaTime;

        if (JustClicked) img.color = clickColor;
        else if (Hover) img.color = hoverColor;
        else img.color = idleColor;
    }

    void Click()
    {
        eventHandler.Click();
        timeSinceLastClick = 0;
    }

    public void OnArduTouchTap()
    {
        Click();
    }

    public void OnArduTouchHover()
    {
        eventHandler.Hover();
        timeSinceLastArduTouchHover = 0;
    }

    public void SetColors(Color idle, Color hover, Color click)
    {
        idleColor = idle;
        hoverColor = hover;
        clickColor = click;
    }
}
