using NextGen.VrManager.PivotManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.VrManager.Ui
{
    public class PivotUiWidget : MonoBehaviour
    {
        public PivotManagerUi manager;
        public Pivot pivot;

        public Image imgIcon;
        public TextMeshProUGUI txtName;

        public void Init(PivotManagerUi _manager, Pivot _pivot)
        {
            manager = _manager;
            pivot = _pivot;

            Draw();
        }
        void Update()
        {
            Draw();
        }

        public void Draw()
        {
            txtName.text = pivot.Name;
        }

        public void OpenInInspector()
        {
            FindObjectOfType<NextGenInspectorUi>().SetupWidgets(pivot);
        }
    }
}
