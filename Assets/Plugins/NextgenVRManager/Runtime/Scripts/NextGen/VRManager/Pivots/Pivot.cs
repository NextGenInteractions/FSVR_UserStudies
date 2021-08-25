using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace NextGen.VrManager.PivotManagement
{
    [RequireComponent(typeof(ParentConstraint))]
    public class Pivot : MonoBehaviour
    {
        public virtual string Name { get { return name; } }

        private bool applicationQuitting = false;

        protected ParentConstraint pc;

        [SerializeField] private bool bindToGlobalPivotOnStart;

        protected virtual void Awake()
        {
            pc = GetComponent<ParentConstraint>();
        }

        public void Start()
        {
            PivotManager.AddPivot(this);

            if (bindToGlobalPivotOnStart)
            {
                if(GlobalPivot.Instance != null)
                    Bind(GlobalPivot.Instance.transform);
            }
        }

        public virtual void OnDestroy()
        {
            if (!applicationQuitting)
                PivotManager.RemovePivot(this);
        }

        public virtual void OnApplicationQuit()
        {
            applicationQuitting = true;
        }

        public void Bind(Transform toFollow)
        {
            int sourceIndex = pc.AddSource(new ConstraintSource() { sourceTransform = toFollow, weight = 1 } );

            pc.SetTranslationOffset(sourceIndex, transform.position - toFollow.position);
            pc.SetRotationOffset(sourceIndex, transform.rotation * Quaternion.Inverse(toFollow.rotation).eulerAngles);

            pc.constraintActive = true;

        }

        public void Unbind(Transform toFollow)
        {
            bool foundSourceToRemove = false;

            for (int i = 0; i < pc.sourceCount; i++)
            {
                if(pc.GetSource(i).sourceTransform == toFollow)
                {
                    pc.RemoveSource(i);
                    foundSourceToRemove = true;
                    break;
                }
            }

            if (!foundSourceToRemove)
                Debug.LogWarning($"Tried to unbind {name} from {toFollow}, but the two weren't bound to begin with.");

            pc.constraintActive = pc.sourceCount > 0;
        }
    }
}

