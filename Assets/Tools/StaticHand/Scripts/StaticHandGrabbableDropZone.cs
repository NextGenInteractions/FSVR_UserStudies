using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StaticHandGrabbableDropZone : MonoBehaviour
{
    private void Start()
    {
        foreach(StaticHand staticHand in FindObjectsOfType<StaticHand>())
        {
            if (!staticHand.allDropZones.Contains(this))
                staticHand.allDropZones.Add(this);
        }
    }
}
