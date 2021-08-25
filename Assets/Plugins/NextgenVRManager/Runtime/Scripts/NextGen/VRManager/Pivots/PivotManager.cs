using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextGen.VrManager.PivotManagement
{
    public class PivotManager : MonoBehaviour
    {
        private static GlobalPivot globalPivot;


        public static GlobalPivot GlobalPivot { get { return globalPivot; } }
        public static Vector3 GlobalPivotPos { get { return globalPivot != null ? globalPivot.transform.position : Vector3.zero; } }
        public static Quaternion GlobalPivotRot { get { return globalPivot != null ? globalPivot.transform.rotation : Quaternion.identity; } }

        public static List<Pivot> pivots = new List<Pivot>();

        public static Action<Pivot> PivotAdded, PivotRemoved;

        void Awake()
        {
        }

        public static void AddPivot(Pivot p)
        {
            if (p.GetType() == typeof(GlobalPivot))
            {
                if (globalPivot == null)
                {
                    globalPivot = p as GlobalPivot;
                }
                else
                {
                    Debug.LogWarning("There are multiple global pivots in the scene. That's just not gonna cut it.");
                }
            }
            else
            {
                pivots.Add(p);
            }
                
            PivotAdded.Invoke(p);
            //p.OnDevicesChanged += UpdatePersistenceMap;
        }

        public static void RemovePivot(Pivot p)
        {
            if (p.GetType() == typeof(GlobalPivot))
            {
                globalPivot = null;
            }
            else
            {
                pivots.Remove(p);
            }

            PivotRemoved.Invoke(p);
            //t.OnDevicesChanged -= UpdatePersistenceMap;
        }

        public static void SetGlobalPivot(Vector3 setPosition, Quaternion setRotation)
        {
            globalPivot.transform.position = setPosition;
            globalPivot.transform.rotation = setRotation;
        }

        public static void AdjustGlobalPivot(Vector3 adjustPosition, Quaternion adjustRotation)
        {

            globalPivot.transform.position += adjustPosition;
            globalPivot.transform.rotation *= adjustRotation;
        }
    }
}

