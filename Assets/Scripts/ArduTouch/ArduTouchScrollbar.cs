using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArduTouchScrollbar : MonoBehaviour, IArduTouchElement
{
    private TouchpadToCanvas touchpad;
    private Scrollbar scrollbar;
    private ScrollRect scrollRect;

    // Start is called before the first frame update
    void Awake()
    {
        Transform check = transform;
        while (check.GetComponent<TouchpadToCanvas>() == null) check = check.parent;
        touchpad = check.GetComponent<TouchpadToCanvas>();
        scrollbar = GetComponent<Scrollbar>();

        for(int i = 0; scrollRect == null && i < transform.parent.childCount; i++)
        {
            Transform child = transform.parent.GetChild(i);
            if (child.GetComponent<ScrollRect>() != null)
                scrollRect = child.GetComponent<ScrollRect>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnArduTouchTap()
    {

    }

    public void OnArduTouchHover()
    {
        //Debug.Log("Scrollbar hovering!");
        Vector3 hoverLocation = touchpad.TweenedCursorLocation;

        Vector3 localLocation = transform.InverseTransformPoint(hoverLocation);

        RectTransform rectTransform = GetComponent<RectTransform>();
        float smaller = localLocation.x + (rectTransform.sizeDelta.x / 2);
        float larger = rectTransform.sizeDelta.x;
        float value = smaller / larger;
        if (value < 0.1f) value = 0;
        if (value > 0.9f) value = 1;

        scrollbar.SetValueWithoutNotify(value);
        scrollRect.content.anchoredPosition = new Vector2((scrollRect.content.sizeDelta.x - scrollRect.viewport.sizeDelta.x) * -value, scrollRect.content.anchoredPosition.y);

    }
}
