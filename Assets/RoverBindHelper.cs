using NextGen.VrManager.PivotManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverBindHelper : MonoBehaviour
{
    public void Bind()
    {
        GlobalPivot.Instance.Bind(transform);
    }

    public void Unbind()
    {
        GlobalPivot.Instance.Unbind(transform);
    }

    public void RoverTeleport()
    {
        Vector3 pos = new Vector3(-33.64157f, 0, 116.4978f);
        Quaternion rot = Quaternion.Euler(0, 93.4796f, 0);

        GlobalPivot.Instance.transform.SetPositionAndRotation(pos, rot);
    }
}
