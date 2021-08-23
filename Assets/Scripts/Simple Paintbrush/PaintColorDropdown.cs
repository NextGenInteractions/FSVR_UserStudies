using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintColorDropdown : MonoBehaviour
{
    public Sprite[] colorIcons;
    public Color color = new Color32(0, 173, 220, 255);

    public void SetColor(int setting)
    {
        Sprite sprite = colorIcons[setting];

        switch(setting)
        {
            case 0:
                color = new Color32(0, 173, 220, 255);
                break;
            case 1:
                color = new Color32(0, 166, 81, 255);
                break;
            case 2:
                color = new Color32(253, 185, 19, 255);
                break;
            case 3:
                color = new Color32(246, 139, 31, 255);
                break;
            case 4:
                color = new Color32(237, 28, 36, 255);
                break;
            case 5:
                color = new Color32(102, 45, 145, 255);
                break;
        }

        GetComponent<Image>().sprite = sprite;
        transform.parent.parent.parent.GetComponent<DeviceWidget>().SetPaintbrushColor(color);
    }
}
