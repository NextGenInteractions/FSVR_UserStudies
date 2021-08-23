using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outputter : MonoBehaviour
{
    public int a;
    public int b;
    public int c;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(a.ToString() + b.ToString() + c.ToString());
    }

    public void SetA(int value)
    {
        a = value;
    }

    public void SetB(int value)
    {
        b = value;
    }

    public void SetC(int value)
    {
        c = value;
    }
}
