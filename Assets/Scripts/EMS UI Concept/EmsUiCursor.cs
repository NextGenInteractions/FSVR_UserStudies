using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmsUiCursor : MonoBehaviour
{
    private TouchpadToCanvas tpc;
    private Image img;
    private RectTransform rect;
    public Vector3 visualOffset;
    private void Awake()
    {
        tpc = FindObjectOfType<TouchpadToCanvas>();
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        img.enabled = tpc.Touching;
        if(img.enabled)
        {
            rect.position = tpc.CursorLocation + visualOffset;
        }
    }
}
