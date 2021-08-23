using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayFrameRate : MonoBehaviour
{

    public float refreshTime = 1.0f;
    private int frameCounter = 0;
    private float timeCounter = 0.0f;
    private float frameRate = 0.0f;

    public TextMeshPro frameRateText;

    // Use this for initialization
    void Start()
    {
        // Not sure why this can't be found. Need to set manually from inspector
        //    TextMeshPro frameRateText = GetComponent<TextMeshPro>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            frameRateText.enabled = !frameRateText.enabled;
        }

        if (timeCounter < refreshTime)
        {
            timeCounter += Time.deltaTime;
            frameCounter++;
        }
        else
        {
            frameRate = (float)frameCounter / timeCounter;
            frameCounter = 0;
            timeCounter = 0.0f;
            frameRateText.SetText("FPS = " + String.Format("{0:0.0}", frameRate));
        }


    }
}
