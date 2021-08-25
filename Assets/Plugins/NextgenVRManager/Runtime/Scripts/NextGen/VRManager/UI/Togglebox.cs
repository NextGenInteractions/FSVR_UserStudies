using UnityEngine;
using NextGen.VrManager.Devices;
using System.Collections.Generic;
using UnityEngine.UI;

namespace NextGen.VrManager.Ui
{
    public class Togglebox : MonoBehaviour
    {
        public bool state;

        public Sprite oSprite;
        public Sprite xSprite;

        private Image icon;
        private Animator anim;

        public void Start()
        {
            icon = transform.GetChild(0).GetComponent<Image>();
            anim = GetComponent<Animator>();
        }

        public void Update()
        {
            icon.sprite = state ? oSprite : xSprite;

            Color fade = new Color32(255, 255, 255, 16);
            icon.color = state ? Color.white : fade;
        }

        public void Toggle()
        {
            state = !state;
            anim.Play("dragAndDropSlotDropin");
        }

    }
}
