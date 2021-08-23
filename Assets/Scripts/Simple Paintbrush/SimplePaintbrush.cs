using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePaintbrush : MonoBehaviour
{
    public float distanceMinimum = 0.025f;
    public Color paintColor;
    private Vector3 lastPaintPos;
    private SimplePaintbrushStroke currentStroke;
    int blobCount = 0;
    public Material strokeMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //TogglePainting();
        }

        if(currentStroke != null)
        {
            if(Vector3.Distance(lastPaintPos,transform.position) > distanceMinimum)
            {
                currentStroke.AddPos(transform.position);
                lastPaintPos = transform.position;
            }
        }
    }

    public void TogglePainting()
    {
        if (currentStroke == null)
        {
            StartPaint();
        }
        else
            EndPaint();
    }

    void StartPaint()
    {
        GameObject stroke = new GameObject("Paintbrush Stroke " + ++blobCount);
        currentStroke = stroke.AddComponent<SimplePaintbrushStroke>();
        currentStroke.Bootup(transform.position, strokeMaterial, paintColor);
        lastPaintPos = transform.position;
    }

    void EndPaint()
    {
        currentStroke = null;
    }
}
