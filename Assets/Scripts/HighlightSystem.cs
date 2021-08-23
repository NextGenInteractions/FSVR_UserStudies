using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapMaterials(GameObject[] obj, Material[] mat)
    {
        foreach (GameObject o in obj)
        {
            MeshRenderer[] renderers = o.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].materials = mat;
            }
        }
    }
}
