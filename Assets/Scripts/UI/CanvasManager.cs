using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text mode;
    public TMP_Text view;
    public GameObject mainEditor;
    GameObject viewCam;

    void Start()
    {
        viewCam = GameObject.Find("ViewCam");
        viewCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void ChangeView(bool is3D)
    {
        view.text = is3D ? "3D View" : "UI View";
        mainEditor.SetActive(!is3D);

        if (viewCam == null)
          return;

        viewCam.SetActive(is3D);
    }

    public void ChangeMode(bool isTest)
    {
        mode.text = isTest ? "Test Mode" : "Runtime";
        mode.color = isTest ? Color.yellow : Color.green;
        LighthouseEditor.instance.isTestMode = isTest;
        LighthouseEditor.instance.GetLighthouses();
    }
}
