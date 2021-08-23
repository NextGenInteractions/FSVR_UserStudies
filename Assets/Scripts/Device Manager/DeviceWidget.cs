using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NextGen;

public class DeviceWidget : MonoBehaviour
{
    public DeviceInstance device;

    [Header("Component References")]
    public TextMeshProUGUI deviceNameText;
    public TextMeshProUGUI deviceBatteryPercentageText;
    public RectTransform deviceBatteryPercentageBar;
    public Text deviceIndexText;
    public TMP_InputField deviceNametagInputField;
    public Text deviceLossCountText;
    public Text deviceMiniLossCountText;
    private SimplePaintbrush paintbrush;

    public Material unlitWhite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Bootup(DeviceInstance _device)
    {
        device = _device;
        Refresh();
        GameObject newPaintbrush = new GameObject(string.Format("Device {0} Paintbrush", device.deviceIndex));
        newPaintbrush.transform.SetParent(GameObject.Find("DevicePaintbrushes").transform);
        paintbrush = newPaintbrush.AddComponent<SimplePaintbrush>();
        newPaintbrush.AddComponent<TrackedObject>().setDevice = device;
        SetPaintbrushColor(new Color32(0, 173, 220, 255));
        paintbrush.strokeMaterial = unlitWhite;
    }

    public void Refresh()
    {
        deviceNameText.text = device.deviceName;
        deviceNameText.color = device.deviceActive ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.25f);
        deviceBatteryPercentageText.gameObject.SetActive(device.HasBattery);
        deviceBatteryPercentageBar.transform.parent.gameObject.SetActive(device.HasBattery);
        deviceBatteryPercentageText.text = device.deviceBatteryPercentage.ToString("P0");
        deviceBatteryPercentageBar.sizeDelta = new Vector2((1 - device.deviceBatteryPercentage) * 37, 20);
        deviceBatteryPercentageText.color = (device.deviceBatteryPercentage > 0.33f) ? Color.white : Color.red;
        deviceBatteryPercentageBar.transform.parent.GetComponent<Image>().color = (device.deviceBatteryPercentage > 0.33f) ? Color.white : Color.red;
        deviceIndexText.text = device.deviceIndex.ToString();
        deviceNametagInputField.text = device.deviceNametag;
        deviceLossCountText.text = device.deviceTrackingLossesCount.ToString();
        deviceMiniLossCountText.text = device.deviceTrackingMiniLossesCount.ToString();
    }

    public void SetNametag(string nametag)
    {
        device.SetNametag(nametag);
    }

    public void SetPaintbrushColor(Color incoming)
    {
        paintbrush.paintColor = incoming;
    }

    public void TogglePaintbrushPainting()
    {
        paintbrush.TogglePainting();
    }

    public void DeletePaintbrushStrokes()
    {

    }
}
