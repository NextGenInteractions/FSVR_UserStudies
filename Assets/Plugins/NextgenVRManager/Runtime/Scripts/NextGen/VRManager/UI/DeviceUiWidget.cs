using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NextGen.VrManager.Devices;
using UnityEngine.EventSystems;
using NextGen.VrManager.DebugTools;

namespace NextGen.VrManager.Ui
{
    public class DeviceUiWidget : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        public DeviceManagerUi manager;
        public Device device;

        public Image imgIcon, imgBatteryLevel, imgBatteryLevelBar;
        public TextMeshProUGUI txtName, txtSerialNumber;

        public float timeSinceLastClick = 0;
        public const float doubleClickInterval = 0.25f;

        public void Init(DeviceManagerUi _manager, Device _device)
        {
            manager = _manager;
            device = _device;

            Draw();
        }
        void Update()
        {
            Draw();

            timeSinceLastClick += Time.deltaTime;
        }

        public void Draw()
        {
            imgIcon.sprite = device.Metadata == null || (int)device.Metadata.Value.Type >= manager.iconSprites.Count ? manager.nullIconSprite : manager.iconSprites[(int)device.Metadata.Value.Type];

            txtName.text = device.DisplayName;

            txtSerialNumber.text = device.Uid;

            if(device.TryGetFeatureValue(CommonDeviceFeatures.batteryLevel, out float getBatteryLevel))
            {
                imgBatteryLevel.gameObject.SetActive(true);

                imgBatteryLevelBar.rectTransform.sizeDelta = new Vector2(19 * getBatteryLevel, 8);
            }
            else
            {
                imgBatteryLevel.gameObject.SetActive(false);
            }
        }

        public void OpenInInspector()
        {
            FindObjectOfType<NextGenInspectorUi>().SetupWidgets(device);
        }

        public void Click()
        {
            OpenInInspector();

            if (timeSinceLastClick < doubleClickInterval)
            {
                TrackedDeviceVisualizer tdv = FindObjectOfType<TrackedDeviceVisualizer>();
                if(tdv.deviceVisualizations.ContainsKey(device))
                {
                    SmartCamera.SetFocus(tdv.deviceVisualizations[device].transform);
                }
            }

            timeSinceLastClick = 0;
        }

        public void OnDrag(PointerEventData data)
        {
        }

        public void OnBeginDrag(PointerEventData data)
        {
            DragAndDropManager.SetDragObject(device);
        }
    }
}
