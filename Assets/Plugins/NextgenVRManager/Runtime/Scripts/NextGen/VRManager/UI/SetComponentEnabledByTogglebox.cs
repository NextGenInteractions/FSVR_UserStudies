using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public class SetComponentEnabledByTogglebox : MonoBehaviour
    {
        public MonoBehaviour component;
        public Togglebox togglebox;

        private void Update()
        {
            component.enabled = togglebox.state;
        }
    }
}
