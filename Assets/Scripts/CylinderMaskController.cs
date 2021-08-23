using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMaskController : MonoBehaviour
{
    [SerializeField]
    Vector2 offset = new Vector2(-0.324f, 0.1242f);
    [SerializeField]
    Transform trans;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector4 vector = mat.GetVector("_Offset");
        Vector3 pos = trans.position;
        if (!Mathf.Approximately(0, vector.x - pos.x) || !Mathf.Approximately(0, vector.y - pos.z))
        {
            vector.x = pos.x + offset.x;
            vector.y = pos.z + offset.y;
            mat.SetVector("_Offset", vector);
        }
    }
}
