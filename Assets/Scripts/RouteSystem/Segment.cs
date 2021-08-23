using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace RouteSystem
{
	[Serializable]
	public class Segment
	{
		public GameObject node;
		public Vector3 nodePos;
		public bool usingObject;
		public float topSpeed;
		[HideInInspector]
		public Vector3 pos
		{
			get
			{
				return usingObject ? node.transform.position : nodePos;
			}
		}
		//public float acceleration;
		//public bool usingAcceleration;
	}
}

