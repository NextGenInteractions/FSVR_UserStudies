using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public abstract class InspectorUiWidget : MonoBehaviour
    {
        protected NextGenInspectorUi nextGenInspectorUi;

        public virtual void Init(NextGenInspectorUi _nextGenInspectorUi)
        {
            nextGenInspectorUi = _nextGenInspectorUi;
        }
    }
}
