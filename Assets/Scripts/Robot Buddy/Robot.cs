using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Robot : MonoBehaviour
{
    public enum Mode
    {
        FlyToNode,
        FixAtGazePoint
    }

    public Mode mode;
    public float flySpeed = 4;

    public Vector3 destination;

    public void Activate()
    {
        Activate(Gazerbeam.GazeFocus.myNode);
    }
    public void Activate(InterfaceNode node)
    {
        switch(mode) {
            case Mode.FlyToNode:
                destination = node.transform.position;
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch(mode)
        {
            case Mode.FlyToNode:
                if (transform.position != destination)
                    transform.position = Vector3.MoveTowards(transform.position, destination, flySpeed * Time.deltaTime);
                break;

            case Mode.FixAtGazePoint:
                if (Gazerbeam.GazeFocus != null)
                {
                    transform.position = Gazerbeam.GazePoint;
                    transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "This fella's name is " + Gazerbeam.GazeFocus.transform.parent.GetComponent<FellaName>().fellaName + "!";
                }
                else
                    transform.position = new Vector3(0, -100, 0);
                break;
        }
    }
}
