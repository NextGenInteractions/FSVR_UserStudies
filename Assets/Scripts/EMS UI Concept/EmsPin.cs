using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmsPin : MonoBehaviour
{
    public bool placed = false;
    public Vector3 placedPosition; 

    private TouchpadToCanvas tpc;
    private Image img;
    private RectTransform rect;
    private FullBodyTransformer fbt;
    private RectTransform textBgRect;
    private RectTransform textRect;

    private Vector3 visualOffset = new Vector3(-0.5f, 0.5f, 0);

    public string textString;
    public TextMeshProUGUI text;

    public TextMeshProUGUI indexText;

    public bool HasText
    {
        get
        {
            return textString != "";
        }
    }

    private void Awake()
    {
        tpc = FindObjectOfType<TouchpadToCanvas>();
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        fbt = FindObjectOfType<FullBodyTransformer>();

        textBgRect = transform.GetChild(0).GetComponent<RectTransform>();
        textRect = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        text = textRect.GetComponent<TextMeshProUGUI>();

        textString = "";
        text.text = textString;
        textBgRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textBgRect.sizeDelta.y);

        indexText.text = (transform.GetSiblingIndex() + 1).ToString();
    }

    private void Update()
    {
        img.enabled = !(!placed && fbt.transitioning > 0);

        if(!placed)
        {
            if (tpc.Touching)
            {
                rect.position = tpc.CursorLocation;
            }
        }

        else
        {
        }

        textBgRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textBgRect.sizeDelta.y);
    }

    public void Place()
    {
        placed = true;
    }

    public void SetText(string toSet)
    {
        textString = toSet;
        //text.text = textString;
    }


}
