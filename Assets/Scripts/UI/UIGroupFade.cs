using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGroupFade : MonoBehaviour
{
    CanvasGroup[] canvasGroups;
    public float duration = 1;
    CanvasGroup currentGroup;
    float currentTime = 1;
    // Start is called before the first frame update
    void Start()
    {
        currentTime = duration;
        canvasGroups = GetComponentsInChildren<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            foreach(CanvasGroup cg in canvasGroups)
            {
                float ratio = currentTime / duration;
                cg.alpha = (cg == currentGroup) ? ratio : (cg.alpha <= 0) ? 0 : 1 - ratio;
            }
        }
    }

    public void ActivateCanvas(string groupName)
    {
        foreach (CanvasGroup cg in canvasGroups)
        {
            if (cg.gameObject.name.Equals(groupName))
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
                currentGroup = cg;
            }
            else
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
        currentTime = 0;
    }
}
