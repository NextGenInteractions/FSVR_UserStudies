using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NextGen.VrManager.ToolManagement;

namespace NextGen.VrManager.Ui
{
    public class ToolUiWidget : MonoBehaviour
    {
        public ToolManagerUi manager;
        public Tool tool;

        public Image imgIcon;
        public TextMeshProUGUI txtName;

        public float timeSinceLastClick = 0;
        public const float doubleClickInterval = 0.25f;

        public void Init(ToolManagerUi _manager, Tool _tool)
        {
            manager = _manager;
            tool = _tool;

            Draw();
        }
        void Update()
        {
            Draw();

            timeSinceLastClick += Time.deltaTime;
        }

        public void Draw()
        {
            txtName.text = tool.Name;
        }

        public void Click()
        {
            OpenInInspector();

            if (timeSinceLastClick < doubleClickInterval)
                SmartCamera.SetFocus(tool.transform);

            timeSinceLastClick = 0;


        }

        public void OpenInInspector()
        {
            FindObjectOfType<NextGenInspectorUi>().SetupWidgets(tool);
        }


    }
}
