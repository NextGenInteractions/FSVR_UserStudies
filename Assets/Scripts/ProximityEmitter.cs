using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ProximityEmitter : MonoBehaviour
{

    public MutiRaeManager.Gas type;

    public float maxValue = 5.0f;
    public float maxDistance = 5.0f;
		public Vector3 dimension = new Vector3();

		[Range(0, 1)]
    public float noiseScale;

    public AnimationCurve curve;

    public bool isEmitting;

		[Header("Debug Setting")]
		public bool showRange;
		public bool showCloud;
		[Range(0.1f, 10)]
		public float cloudDensity = 0.1f;
		public Color edgeColor = new Color(0.5f,0.5f,0.5f,0.2f), dangerColor = Color.yellow, deadlyColor = Color.red;
		private Vector3[] points = new Vector3[8];

	public float getValue(Transform t)
    {
        if (!isEmitting || !InRange(t.position))
        {
            return 0;
        }
        float distance = Vector3.Distance(t.position, transform.position);

        float noise = Random.Range(-noiseScale, noiseScale) * maxValue;

        return  Mathf.Clamp( noise + maxValue * (curve.Evaluate(1 - (distance * 2/dimension.magnitude))), 0, maxValue);
    }

	private bool InRange (Vector3 pos)
	{
		Vector3 localPos = transform.InverseTransformPoint(pos);
		return Mathf.Abs(localPos.x) - Mathf.Abs(dimension.x) < 0 && Mathf.Abs(localPos.y) - Mathf.Abs(dimension.y) < 0 && Mathf.Abs(localPos.z) - Mathf.Abs(dimension.z) < 0;
	}
	/*
			public float getValue()
			{
					return getValue(testObj);
			}
	*/

	private void OnDrawGizmos()
	{
#if UNITY_EDITOR
		if (showRange)
		{
			Gizmos.color = edgeColor;
			points[0] = transform.TransformPoint(dimension.x / 2, dimension.y / 2, dimension.z / 2);
			points[1] = transform.TransformPoint(dimension.x / 2, dimension.y / 2, -dimension.z / 2);
			points[2] = transform.TransformPoint(dimension.x / 2, -dimension.y / 2, -dimension.z / 2);
			points[3] = transform.TransformPoint(dimension.x / 2, -dimension.y / 2, dimension.z / 2);
			points[4] = transform.TransformPoint(-dimension.x / 2, dimension.y / 2, dimension.z / 2);
			points[5] = transform.TransformPoint(-dimension.x / 2, dimension.y / 2, -dimension.z / 2);
			points[6] = transform.TransformPoint(-dimension.x / 2, -dimension.y / 2, -dimension.z / 2);
			points[7] = transform.TransformPoint(-dimension.x / 2, -dimension.y / 2, dimension.z / 2);
			for (int i = 0; i < 4; ++i)
			{
				Gizmos.DrawLine(points[i], points[(i + 1) % 4]);
				Gizmos.DrawLine(points[i + 4], points[(i + 1) % 4 + 4]);
				Gizmos.DrawLine(points[i], points[i + 4]);
			}
		}
		if (showCloud)
		{
			for (float x = dimension.x; x > -dimension.x; x -= cloudDensity)
			{
				for (float y = dimension.y; y > -dimension.y; y -= cloudDensity)
				{
					for (float z = dimension.z; z > -dimension.z; z -= cloudDensity)
					{
						Vector3 dot = transform.TransformPoint(x / 2, y / 2, z / 2);
						float distance = Vector3.Distance(dot, transform.position);
						float noise = Random.Range(-noiseScale, noiseScale);
						Gizmos.color = Color.Lerp(dangerColor, deadlyColor, noise + 1 - curve.Evaluate(distance * 2 / dimension.magnitude));
						Gizmos.DrawWireCube(dot, Vector3.one * 0.05f);
					}
				}
			}
		}
#endif
	}
}
