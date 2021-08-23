using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackspaceToReturn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            //Set all screens to inactive, except for the one screen specified as the "start screen" in the Inspector.
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
            GetComponent<TouchpadToCanvas>().startingScreen.SetActive(true);
        }
    }
}
