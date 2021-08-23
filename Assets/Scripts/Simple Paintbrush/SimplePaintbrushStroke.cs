using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePaintbrushStroke : MonoBehaviour
{
    LineRenderer lineRend;

    // Start is called before the first frame update
    public void Bootup(Vector3 _startPos, Material _material, Color _paintColor)
    {
        lineRend = gameObject.AddComponent<LineRenderer>();
        lineRend.startColor = Color.white;
        lineRend.endColor = Color.white;
        lineRend.material = _material;
        lineRend.material.color = _paintColor;
        lineRend.startWidth = 0.025f;
        lineRend.endWidth = 0.025f;
        lineRend.positionCount = 1;
        lineRend.SetPosition(0, _startPos);
    }

    public void AddPos(Vector3 _pos)
    {
        lineRend.positionCount++;
        lineRend.SetPosition(lineRend.positionCount - 1, _pos);
    }
}
