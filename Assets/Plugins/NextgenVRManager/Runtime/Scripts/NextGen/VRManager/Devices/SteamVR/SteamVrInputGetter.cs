using UnityEngine;
using Valve.VR;

namespace NextGen.VrManager.Devices.SteamVr
{
    public class SteamVrInputGetter : MonoBehaviour
    {
        public SteamVR_Input_Sources device;

        private SteamVR_ActionSet set;

        public bool trigger;
        public bool touchpad;
        public bool grip;

        [Range(0, 1)] public float triggerSingle;

        //public SteamVR_Behaviour_Boolean boolean;
        private SteamVR_Action_Boolean triggerAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
        private SteamVR_Action_Boolean touchpadAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");
        private SteamVR_Action_Boolean gripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

        private SteamVR_Action_Single triggerSingleAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");


        // Start is called before the first frame update
        void Start()
        {
            //set = SteamVR_Input.GetActionSet("default");
            //set.Activate();

            //boolean.onPressDown.AddListener(TriggerDown);
            //boolean.onPressUp.AddListener(TriggerUp);

            //action.onStateDown += TriggerDownAct;
            //action.onStateUp += TriggerUpAct;
        }

        private void TriggerDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources sources, bool value)
        {
            Debug.Log("Down");
        }

        private void TriggerUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources sources, bool value)
        {
            Debug.Log("Up");
        }

        private void TriggerDownAct(SteamVR_Action_Boolean action, SteamVR_Input_Sources sources)
        {
            Debug.Log("DOWN");
        }

        private void TriggerUpAct(SteamVR_Action_Boolean action, SteamVR_Input_Sources sources)
        {
            Debug.Log("UP");
        }

        // Update is called once per frame
        void Update()
        {
            trigger = triggerAction.GetState(device);
            touchpad = touchpadAction.GetState(device);
            grip = gripAction.GetState(device);

            triggerSingle = triggerSingleAction.GetAxis(device);
        }

        public void DebugState(bool yeet)
        {
            if (yeet)
                Debug.Log("Neat");
            else
                Debug.Log("Note");
        }

        public void DebugState(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources sources, SteamVR_Behaviour_Boolean value)
        {
            if (value)
                Debug.Log("Neat");
            else
                Debug.Log("Note");
        }
    }
}