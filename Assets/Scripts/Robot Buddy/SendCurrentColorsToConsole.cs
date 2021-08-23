using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SendCurrentColorsToConsole : MonoBehaviour
{
    public TextMeshProUGUI outputText;

    ColorBox[] colorBoxes;
    List<ColorBox.BoxColor> colorsSoFar = new List<ColorBox.BoxColor>();

    // Start is called before the first frame update
    void Start()
    {
        colorBoxes = GameObject.FindObjectsOfType<ColorBox>();
    }

    // Update is called once per frame
    void Update()
    {
        colorsSoFar.Clear();
        foreach(ColorBox colorBox in colorBoxes)
        {
            if(colorBox.inView)
            {
                if(!colorsSoFar.Contains(colorBox.boxColor))
                {
                    colorsSoFar.Add(colorBox.boxColor);
                }
            }
        }
        string colorsSoFarString = "Colors:\n";
        if (colorsSoFar.Count == 0) colorsSoFarString += "None";
        foreach(ColorBox.BoxColor boxColor in colorsSoFar)
        {
            colorsSoFarString += boxColor.ToString() + "\n";
        }
        outputText.text = colorsSoFarString;
  
    }
}
