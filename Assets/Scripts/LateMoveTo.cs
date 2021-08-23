using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateMoveTo : MonoBehaviour
{
    [Range(0, 1)]
    public float weight;
    public Transform transformToMove;
    public Transform target;

    private void LateUpdate()
    {
        transformToMove.position = Vector3.Lerp(transformToMove.position, target.position, weight);
        transformToMove.rotation = Quaternion.Lerp(transformToMove.rotation, target.rotation, weight);
    }
}
