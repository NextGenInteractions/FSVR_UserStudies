using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleThinker : MonoBehaviour
{
    public float fov;

    public bool clickToThink = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(clickToThink)
        {
            clickToThink = false;
            TriangleThink();
        }
    }

    void TriangleThink()
    {
        for(int x = 0; x < 10; x++)
        {
            float width = (4 * Mathf.Pow(x, 2)) + Mathf.Pow(x, 2);
            Debug.Log(string.Format("60 FOV. {0} out, {1} HorizonWidth", x, width));
        }
    }
}
