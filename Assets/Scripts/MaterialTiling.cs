using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTiling : MonoBehaviour
{
    public enum Axis
    {
        Both,
        X,
        Y
    }
    public Axis appliedAxis = Axis.X;
    public float speed = 5;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 tile = mat.GetTextureOffset("_BaseMap");
//        Debug.Log(tile);
        if (appliedAxis != Axis.X)
            tile.y += speed;
        if (appliedAxis != Axis.Y)
            tile.x += speed;
        mat.SetTextureOffset("_BaseMap", tile);
    }
}
