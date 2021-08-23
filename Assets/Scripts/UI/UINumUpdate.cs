using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINumUpdate : MonoBehaviour
{
    public Text UItext;
    public string postfix = "", suffix = "";
    public string toStringOption = "0";
    // Start is called before the first frame update
    void Start()
    {
        if (UItext == null)
            UItext = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NumberUpdate(float input)
    {
        UItext.text = suffix + input.ToString(toStringOption) + postfix;
    }

    public void NumberUpdate(int input)
    {
        UItext.text = suffix + input.ToString(toStringOption) + postfix;
    }

    public void TextToNumber(string input)
    {

    }
}
