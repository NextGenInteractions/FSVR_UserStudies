using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextGen.VrManager.Devices;
using TMPro;
using UnityEngine.UI;
using NextGen.VrManager.DebugTools;

namespace NextGen.VrManager.Ui
{
    public class JitterLossCountUiWidget : InspectorUiWidget
    {
        private JitterLossCount jlc;
        public TextMeshProUGUI text;
        public Button button;

        public void Awake()
        {
            button.onClick.AddListener(ResetClick);
        }

        public override void Init(NextGenInspectorUi _nextGenInspectorUi)
        {
            base.Init(_nextGenInspectorUi);

            jlc = JitterLossCounter.GetJitterLossCount(nextGenInspectorUi.Device);
        }

        public void Update()
        {
            text.text = $"J: {jlc.jitters}, L: {jlc.losses}, TSL: {jlc.timeSinceLastTrack:F1}";
        }

        private void ResetClick()
        {
            jlc.Reset();
        }
    }
}
