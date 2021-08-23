using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    GameObject Highlight;
    public Material highlightMaterial;


    private void Awake()
    {
        generateMesh();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enableHighlight()
    {
        Highlight.SetActive(true);
    }

    public void disableHighlight()
    {
        Highlight.SetActive(false);
    }


    void generateMesh()
    {
        Highlight = Instantiate(this.gameObject);
        Highlight.transform.SetParent(this.transform, true);
        SwapMaterials(Highlight, highlightMaterial);
    }

    public void SwapMaterials(GameObject obj, Material mat)
    {
        MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < renderers[i].materials.Length; j++)
            { 
                renderers[i].materials[j] = mat;
            }
        }
    }

}
