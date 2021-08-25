using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NextGen.Tools.Ui
{
    public class Toolbar : MonoBehaviour
    {
        public void ToggleWindow(CanvasGroup window)
        {
            window.alpha = window.alpha == 1 ? 0 : 1;
            window.interactable = !window.interactable;
            window.blocksRaycasts = !window.blocksRaycasts;
        }
    }
}
