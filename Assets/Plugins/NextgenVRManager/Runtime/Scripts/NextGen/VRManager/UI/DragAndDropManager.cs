using NextGen.VrManager.Devices;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public class DragAndDropManager : MonoBehaviour
    {
        public static DragAndDropManager Instance;

        public Device dragObject;
        public DragAndDropSlot slotToDrop;

        private void Awake()
        {
            Instance = this;
        }

        public static void SetDragObject(Device _dragObject)
        {
            Instance.dragObject = _dragObject;
        }

        public static void SetSlotToDrop(DragAndDropSlot _slotToDrop)
        {
            Instance.slotToDrop = _slotToDrop;
        }

        private void Update()
        {
            bool lmbPressed;

            lmbPressed = UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;

            if (lmbPressed)
            {
                if (slotToDrop != null)
                {
                    slotToDrop.InvokeAction(dragObject);
                    slotToDrop = null;
                }
            }
            else
            {
                if (dragObject != null)
                {
                    dragObject = null;
                }
            }
        }
    }
}

