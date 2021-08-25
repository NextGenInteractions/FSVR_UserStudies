using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraManager
{
    public static void SetFocus(Transform focus)
    {
        Debug.Log($"The focus has been set to {focus}.");
        CameraController.Instance.target = focus;
    }
}
