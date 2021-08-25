using NextGen.VrManager.PivotManagement;
using System;
using TMPro;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public class PivotInspectorUiWidget : MonoBehaviour
    {
        public Pivot pivot;

        private NextGenInspectorUi inspector;

        [SerializeField] private TextMeshProUGUI posText;
        [SerializeField] private TextMeshProUGUI rotText;

        public void Init(NextGenInspectorUi _inspector)
        {
            inspector = _inspector;
        }

        public void Update()
        {
            posText.text = $"Position: {inspector.Pivot.transform.position.ToString()}";
            rotText.text = $"Rotation: {inspector.Pivot.transform.rotation.ToString()}";
        }
    }
}

