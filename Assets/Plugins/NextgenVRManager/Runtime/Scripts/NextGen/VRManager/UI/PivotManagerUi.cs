using NextGen.VrManager.PivotManagement;
using NextGen.VrManager.ToolManagement;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public class PivotManagerUi : MonoBehaviour
    {
        [SerializeField] private PivotUiWidget pivotUiWidget;
        [SerializeField] private Transform widgetParent;

        private static Dictionary<Type, ToolInspectorUiWidget> _typeToWidgetMap = new Dictionary<Type, ToolInspectorUiWidget>();
        public static IReadOnlyDictionary<Type, ToolInspectorUiWidget> TypeToWidgetMap { get { return _typeToWidgetMap; } }

        private List<PivotUiWidget> widgets = new List<PivotUiWidget>();

        private void OnEnable()
        {
            PivotManager.PivotAdded += AddPivotWidgetForPivot;
            PivotManager.PivotRemoved += RemovePivotWidgetForPivot;
        }

        public void Awake()
        {
        }

        private void AddPivotWidgetForPivot(Pivot p)
        {
            PivotUiWidget widget = Instantiate(pivotUiWidget);
            widget.Init(this, p);

            widget.transform.SetParent(widgetParent);
            widget.name = p.Name;

            widgets.Add(widget);
        }

        private void RemovePivotWidgetForPivot(Pivot p)
        {
            widgets.ForEach((widget) =>
            {
                if (widget.pivot.Equals(p))
                {
                    Destroy(widget.gameObject);
                }
            });
        }
    }
}
