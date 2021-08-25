using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public class Dialog : MonoBehaviour
    {
        private bool applicationQuitting = false;

        public virtual void Start()
        {
            DialogManager.AddDialog(this);
            GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }

        public virtual void OnDestroy()
        {
            if (!applicationQuitting)
                DialogManager.RemoveDialog(this);
        }

        public virtual void OnApplicationQuit()
        {
            applicationQuitting = true;
        }
    }
}
