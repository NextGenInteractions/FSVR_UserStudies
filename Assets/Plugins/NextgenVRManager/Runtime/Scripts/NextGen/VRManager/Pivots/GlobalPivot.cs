using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace NextGen.VrManager.PivotManagement
{
    public class GlobalPivot : Pivot
    {
        public override string Name { get { return "Global Pivot"; } }

        public static GlobalPivot Instance;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }
    }
}

