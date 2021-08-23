using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBox : MonoBehaviour
{
    public enum BoxColor
    {
        Red,
        Blue,
        Green,
        Yellow
    }


    Camera robotCam;

    public BoxColor boxColor;
    private BoxColor lastBoxColor;

    private Renderer rend;

    public bool inView;

    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        lastBoxColor = boxColor;
        rend.material.color = GetColor(boxColor);
        robotCam = GameObject.Find("Robot").GetComponentInChildren<Camera>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boxColor != lastBoxColor)
            rend.material.color = GetColor(boxColor);

        lastBoxColor = boxColor;

        inView = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(robotCam), col.bounds);
    }

    Color GetColor(BoxColor color)
    {
        Color toRet = Color.white;
        switch(color)
        {
            case BoxColor.Red:
                toRet = Color.red;
                break;
            case BoxColor.Blue:
                toRet = Color.blue;
                break;
            case BoxColor.Green:
                toRet = Color.green;
                break;
            case BoxColor.Yellow:
                toRet = Color.yellow;
                break;
        }
        return toRet;
    }
}
