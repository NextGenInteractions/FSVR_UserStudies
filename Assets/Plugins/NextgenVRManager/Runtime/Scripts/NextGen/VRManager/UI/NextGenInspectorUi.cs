using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;
using UnityEngine.UI;
using TMPro;
using NextGen.VrManager.PivotManagement;

namespace NextGen.VrManager.Ui
{
    public class NextGenInspectorUi : MonoBehaviour
    {
        public GameObject jitterLossCountWidget;
        public GameObject setMetadataWidget;

        public List<NameToWidget> featureWidgetPrefabs = new List<NameToWidget>();
        public PivotInspectorUiWidget pivotInspectorUiWidget;

        private Dictionary<string, GameObject> featureWidgetPrefabsDictionary = new Dictionary<string, GameObject>();

        public Transform widgetParent;
        public TextMeshProUGUI labelText;

        public object item;
        public Device Device { get { return item as Device; } }
        public Tool Tool { get { return item as Tool; } }
        public Pivot Pivot { get { return item as Pivot; } }

        private int redrawLayout = 0;

        public void Awake()
        {
            foreach (NameToWidget ntw in featureWidgetPrefabs)
                featureWidgetPrefabsDictionary[ntw.name] = ntw.widget;
        }

        public void SetupWidgets(Device _device)
        {
            item = _device;
            Device device = (Device)item;

            ClearWidgets();

            labelText.text = device.DisplayName;

            if (device.GetFeatures().Contains((DeviceFeatureUsage)CommonDeviceFeatures.isTracked))
                Instantiate(jitterLossCountWidget, widgetParent).GetComponent<InspectorUiWidget>().Init(this);

            foreach (DeviceFeatureUsage feature in device.GetFeatures())
            {
                if (featureWidgetPrefabsDictionary.ContainsKey(feature.name))
                {
                    Instantiate(featureWidgetPrefabsDictionary[feature.name], widgetParent).GetComponent<InspectorUiWidget>().Init(this);
                }
            }

            Instantiate(setMetadataWidget, widgetParent).GetComponent<InspectorUiWidget>().Init(this);

            redrawLayout = 2;
        }

        public void SetupWidgets(Tool _tool)
        {
            item = _tool;
            Tool tool = (Tool)item;

            ClearWidgets();

            labelText.text = tool.Name;
            
            if(ToolManagerUi.TypeToWidgetMap.ContainsKey(tool.GetType()))
                Instantiate(ToolManagerUi.TypeToWidgetMap[tool.GetType()], widgetParent).Init(this);
            else
                Debug.LogWarning($"Could not find a widget for tool of type: {tool.GetType()}. Is this intentional?");

            redrawLayout = 2;
        }

        public void SetupWidgets(Pivot _pivot)
        {
            item = _pivot;
            Pivot pivot = (Pivot)item;

            ClearWidgets();

            labelText.text = pivot.Name;

            Instantiate(pivotInspectorUiWidget, widgetParent).Init(this);

            redrawLayout = 2;
        }

        public void LateUpdate()
        {
            if(redrawLayout > 0)
            {
                redrawLayout--;
                LayoutRebuilder.ForceRebuildLayoutImmediate(widgetParent.GetComponent<RectTransform>());
            }
        }

        void ClearWidgets()
        {
            foreach (InspectorUiWidget featureUiWidget in widgetParent.GetComponentsInChildren<InspectorUiWidget>())
            {
                Destroy(featureUiWidget.gameObject);
            }

            foreach (PivotInspectorUiWidget widget in widgetParent.GetComponentsInChildren<PivotInspectorUiWidget>())
                Destroy(widget.gameObject);

        }

        [System.Serializable]
        public struct NameToWidget
        {
            public string name;
            public GameObject widget;
        }
    }
}
