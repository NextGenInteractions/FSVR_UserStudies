using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData data)
    {
        transform.position = data.position;
        transform.parent.SetSiblingIndex(transform.parent.parent.childCount - 2);
    }
}
