using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoDebugTester : MonoBehaviour
{
    public Material litDown;
    public Material litUp;

    public Renderer[] cubes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QA()
    {
        Debug.Log("A");
        LightUpCube(0);
    }

    public void QB()
    {
        Debug.Log("B");
        LightUpCube(1);
    }

    public void QC()
    {
        Debug.Log("C");
        LightUpCube(2);
    }

    public void QD()
    {
        Debug.Log("D");
        LightUpCube(3);
    }

    public void LightUpCube(int q)
    {
        foreach(Renderer cube in cubes)
        {
            cube.material = litDown;
        }
        cubes[q].material = litUp;
    }
}
