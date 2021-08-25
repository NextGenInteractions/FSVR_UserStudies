using NextGen.VrManager.ToolManagement;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public class ToolManagerUi : MonoBehaviour
    {
        public GameObject ToolUiWidget;
        public Transform WidgetParent;

        private static Dictionary<Type, ToolInspectorUiWidget> _typeToWidgetMap = new Dictionary<Type, ToolInspectorUiWidget>();
        public static IReadOnlyDictionary<Type, ToolInspectorUiWidget> TypeToWidgetMap { get { return _typeToWidgetMap; } }

        public List<ToolUiWidget> Widgets = new List<ToolUiWidget>();

        private void OnEnable()
        {
            ToolManager.ToolAdded += AddToolWidgetForTool;
            ToolManager.ToolRemoved += RemoveToolWidgetForTool;
        }

        public void Awake()
        {
            ToolInspectorUiWidget[] list = Resources.LoadAll<ToolInspectorUiWidget>("");
            foreach (ToolInspectorUiWidget widget in list)
            {
                _typeToWidgetMap[widget.ToolType] = widget;
            }
        }

        private void AddToolWidgetForTool(ToolManagement.Tool t)
        {
            ToolUiWidget widget = Instantiate(ToolUiWidget).GetComponent<ToolUiWidget>();
            widget.Init(this, t);

            widget.transform.SetParent(WidgetParent);
            widget.name = t.Name;

            Widgets.Add(widget);
        }

        private void RemoveToolWidgetForTool(ITool t)
        {
            Widgets.ForEach((widget) =>
            {
                if (widget.tool.Equals(t))
                {
                    Destroy(widget.gameObject);
                }
            });
        }
    }
}
