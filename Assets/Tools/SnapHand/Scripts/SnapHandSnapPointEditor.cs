using Leap.Unity;
using RootMotion.FinalIK;
using System.Linq;
using UnityEngine;

namespace NextGen.Tools
{
    public class SnapHandSnapPointEditor : MonoBehaviour
    {
        [SerializeField]
        bool viewHand = false;

        [SerializeField]
        SnapHandSnapZone snapPoint;

        [SerializeField]
        public CcdFingerRig handIKLeft, handIKRight;
        public Transform rootLeft, rootRight;

        private void Awake()
        {
            viewHand = false;
        }

        private void Update()
        {
            if (viewHand)
            {
                if (snapPoint.handedness == SnapHand.Handedness.None)
                {
                    handIKLeft.gameObject.SetActive(false);
                    handIKRight.gameObject.SetActive(false);
                }
                else if (snapPoint.handedness == SnapHand.Handedness.Left)
                {
                    handIKLeft.gameObject.SetActive(true);
                    handIKRight.gameObject.SetActive(false);

                    //rootLeft.SetPositionAndRotation(snapPoint.handTargets.root.target.position, snapPoint.handTargets.root.target.rotation);
                }
                else if (snapPoint.handedness == SnapHand.Handedness.Right)
                {
                    handIKLeft.gameObject.SetActive(false);
                    handIKRight.gameObject.SetActive(true);

                    //rootRight.SetPositionAndRotation(snapPoint.handTargets.root.target.position, snapPoint.handTargets.root.target.rotation);
                }
            }
            else
            {
                if(handIKLeft)
                {
                    handIKLeft.gameObject.SetActive(false);
                }
                if(handIKRight)
                {
                    handIKRight.gameObject.SetActive(false);
                }
            }
        }
    }
}
