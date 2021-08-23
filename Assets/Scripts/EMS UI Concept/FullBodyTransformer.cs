using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullBodyTransformer : MonoBehaviour
{
    public Vector3[] locations;
    public Vector3[] scales;

    public int setToSet;

    public float transitioning = 0;

    private Vector3 lastTargetLocation;
    private Vector3 lastTargetScale;

    private Vector3 targetLocation;
    private Vector3 targetScale;

    private RectTransform rectTransform;

    private void Awake()
    {
        lastTargetLocation = locations[0];
        lastTargetScale = scales[0];

        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(setToSet != 0)
        {
            locations[setToSet] = GetComponent<RectTransform>().anchoredPosition;
            scales[setToSet] = GetComponent<RectTransform>().localScale;

            setToSet = 0;
        }

        if(transitioning > 0)
        {
            if(transitioning < 1)
            {
                rectTransform.anchoredPosition = Vector3.Lerp(lastTargetLocation, targetLocation, transitioning);
                rectTransform.localScale = Vector3.Lerp(lastTargetScale, targetScale, transitioning);

                transitioning += Time.deltaTime * 3;
            }
            else
            {
                rectTransform.anchoredPosition = targetLocation;
                rectTransform.localScale = targetScale;

                lastTargetLocation = targetLocation;
                lastTargetScale = targetScale;

                transitioning = 0;
            }
        }
    }

    public void Set(int index)
    {
        if (transitioning == 0)
        {
            targetLocation = locations[index];
            targetScale = scales[index];
            transitioning += Time.deltaTime;
        }

        /*
        targetLocation = locations[index];
        targetScale = scales[index];

        rectTransform.anchoredPosition = targetLocation;
        rectTransform.localScale = targetScale;
        */

    }
}
