using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceNode : MonoBehaviour
{

    public bool inView;
    private Collider col;
    private Renderer rend;
    private Canvas canvas;

    public Material[] mats;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        canvas = transform.GetChild(0).GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        inView = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main),col.bounds);
        if (inView && canvas != null)
        {
            canvas.transform.LookAt(Camera.main.transform);
            canvas.transform.Rotate(new Vector3(0, 180, 0));
        }
    }

    public void Set(bool set)
    {
        rend.material = mats[set ? 1 : 0];
        if(canvas != null) canvas.gameObject.SetActive(set);
    }
}
