using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : Button
{
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    public void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();

        Debug.Log("PRESSIN");
    }
}
