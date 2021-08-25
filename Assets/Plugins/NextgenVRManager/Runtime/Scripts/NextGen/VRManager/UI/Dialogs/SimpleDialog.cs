using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NextGen.VrManager.Ui
{
    public class SimpleDialog : Dialog
    {
        public TextMeshProUGUI messageText;

        public override void Start()
        {
            base.Start();

            GetComponentInChildren<Button>().onClick.AddListener(DestroySelf);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        public void SetMessage(string message)
        {
            messageText.text = message;
        }
    }
}
