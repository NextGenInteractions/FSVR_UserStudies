using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NextGen;

public class DeviceManagerUI : MonoBehaviour
{
    public bool utilityVisiblity = false;
    public bool controllerVisibility = true;

    [Header("Component References")]
    public Image utilityToggleIcon;
    public Image controllerToggleIcon;

    public Transform deviceWidgets;

    // Start is called before the first frame update
    public void Bootup()
    {
        if(deviceWidgets == null) deviceWidgets = transform.Find("DevicesUI");

        SetUtilityVisibility(utilityVisiblity);
        SetControllerVisibility(controllerVisibility);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleUtilityVisibility()
    {
        utilityVisiblity = !utilityVisiblity;
        SetUtilityVisibility(utilityVisiblity);
    }

    public void ToggleControllerVisiblity()
    {
        controllerVisibility = !controllerVisibility;
        SetControllerVisibility(controllerVisibility);
    }

    public void SetUtilityVisibility(bool visible)
    {
        utilityToggleIcon.color = visible ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0.5f);
        for(int i = 0; i < deviceWidgets.childCount; i++)
        {
            DeviceWidget deviceWidget = deviceWidgets.GetChild(i).GetComponent<DeviceWidget>();

            if(deviceWidget.device.GetDeviceCategory == DeviceInstance.DeviceCategory.Utility)
            {
                deviceWidget.gameObject.SetActive(visible);
            }
        }
    }

    public void SetControllerVisibility(bool visible)
    {
        controllerToggleIcon.color = visible ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
        for (int i = 0; i < deviceWidgets.childCount; i++)
        {
            DeviceWidget deviceWidget = deviceWidgets.GetChild(i).GetComponent<DeviceWidget>();

            if (deviceWidget.device.GetDeviceCategory == DeviceInstance.DeviceCategory.Controller)
            {
                deviceWidget.gameObject.SetActive(visible);
            }
        }


    }
}
