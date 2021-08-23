using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazerbeamCatcher : MonoBehaviour
{
    public InterfaceNode myNode;
    public bool catching;

    public Material catchingMat;
    public Material notCatchingMat;

    // Start is called before the first frame update
    void Start()
    {
        myNode = transform.parent.GetComponent<InterfaceNode>();
    }

    // Update is called once per frame
    void Update()
    {
        catching = Gazerbeam.GazeFocus == this;
        if(myNode != null) myNode.Set(catching);
    }
}
