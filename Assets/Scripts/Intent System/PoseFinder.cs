using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseFinder : MonoBehaviour
{

    [SerializeField] private Transform visualizations;
    [SerializeField] private Material renderLineMaterial;

    [Header("Tracked Points")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform lHand;
    [SerializeField] private Transform rHand;
    [SerializeField] private Transform multiRae;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(multiRae == null)
        {
            multiRae = FindObjectOfType<IsAMultiRae>().transform;
        }

        ClearVisualizations();

        foreach(RaycastHit hit in Physics.RaycastAll(head.position, Vector3.down))
        {
            if(hit.transform.name == "Ground")
                RenderLine(head.position, hit.point, Color.green);
        }

    }

    public void ClearVisualizations()
    {
        foreach (LineRenderer lineRenderer in visualizations.GetComponentsInChildren<LineRenderer>())
            Destroy(lineRenderer.gameObject);
    }

    public void RenderLine(Vector3 a, Vector3 b, Color color)
    {
        if(visualizations != null)
        {
            GameObject obj = new GameObject("LineRenderer");
            obj.transform.SetParent(visualizations);

            LineRenderer line = obj.AddComponent<LineRenderer>();

            line.material = renderLineMaterial;
            line.material.color = color;
            line.startWidth = line.endWidth = 0.05f;

            Vector3[] linePoints = new Vector3[2];

            linePoints[0] = a;
            linePoints[1] = b;

            line.SetPositions(linePoints);
        }
    }
}
