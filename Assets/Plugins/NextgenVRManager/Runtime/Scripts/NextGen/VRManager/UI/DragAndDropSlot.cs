using NextGen.VrManager.Devices;
using NextGen.VrManager.ToolManagement;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NextGen.VrManager.Ui
{
    public class DragAndDropSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<Device> deviceSet;

        private Animator anim;

        public DeviceSlot slotRequirements;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void OnPointerEnter(PointerEventData data)
        {
            DragAndDropManager.SetSlotToDrop(this);
        }

        public void OnPointerExit(PointerEventData data)
        {
            DragAndDropManager.SetSlotToDrop(null);
        }

        public void InvokeAction(Device d)
        {
            bool hasAllRequirements = true;
            foreach(DeviceFeatureUsage feature in slotRequirements.RequiredFeatures)
            {
                if (!d.GetFeatures().Contains(feature))
                    hasAllRequirements = false;
            }

            if(hasAllRequirements)
            {
                anim.Play("dragAndDropSlotDropin");
                deviceSet?.Invoke(d);
            }
            else
            {
                anim.Play("dragAndDropSlotRefuse");
            }
        }
    }
}

