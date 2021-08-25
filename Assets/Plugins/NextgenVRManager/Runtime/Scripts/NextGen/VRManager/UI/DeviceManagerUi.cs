using UnityEngine;
using NextGen.VrManager.Devices;
using System.Collections.Generic;

namespace NextGen.VrManager.Ui
{
    public class DeviceManagerUi : MonoBehaviour
    {
        public static DeviceManagerUi Instance;

        public GameObject DeviceUiWidget;
        public Transform WidgetParent;

        public List<DeviceUiWidget> Widgets = new List<DeviceUiWidget>();

        public List<Sprite> iconSprites;
        public Sprite nullIconSprite;

        private bool applicationQuitting = false;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            foreach (Device device in DeviceManager.ActiveDevices)
            {
                AddDeviceWidgetForDevice(device);
            }

            DeviceManager.DeviceAdded += AddDeviceWidgetForDevice;
            DeviceManager.DeviceRemoved += RemoveDeviceWidgetForDevice;
        }

        private void OnDisable()
        {
            if(!applicationQuitting)
            {
                Widgets.ForEach((widget) => { if (widget.gameObject != null) Destroy(widget.gameObject); });

                DeviceManager.DeviceAdded -= AddDeviceWidgetForDevice;
                DeviceManager.DeviceRemoved -= RemoveDeviceWidgetForDevice;
            }
        }

        private void OnApplicationQuit()
        {
            applicationQuitting = true;
        }

        private void AddDeviceWidgetForDevice(Device h)
        {
            DeviceUiWidget widget = Instantiate(DeviceUiWidget).GetComponent<DeviceUiWidget>();
            widget.Init(this, h);

            widget.transform.SetParent(WidgetParent);
            widget.name = h.Uid + " " + h.Name;

            Widgets.Add(widget);
        }

        private void RemoveDeviceWidgetForDevice(Device h)
        {
            Widgets.ForEach((widget) =>
            {
                if(widget.device.Equals(h))
                {
                    Destroy(widget.gameObject);
                }
            });
        }

        public static Sprite GetIconSpriteForDevice(Device d)
        {
            if (d.Metadata == null || (int)d.Metadata.Value.Type >= Instance.iconSprites.Count)
                return Instance.nullIconSprite;
            else
                return Instance.iconSprites[(int)d.Metadata.Value.Type];
        }
    }
}
