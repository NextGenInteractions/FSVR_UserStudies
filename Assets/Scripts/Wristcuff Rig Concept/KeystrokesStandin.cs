using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeystrokesStandin : MonoBehaviour
{
    public Material litDown;
    public Material litUp;

    public Renderer leftBumper;
    public Renderer rightBumper;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        anim.SetBool("PhysicalButtonInput", left || right);
        anim.SetBool("PhysicalButtonLeft", left);
        anim.SetBool("PhysicalButtonRight", right);

        if (left) leftBumper.material = litUp;
        else leftBumper.material = litDown;

        if(right) rightBumper.material = litUp;
        else rightBumper.material = litDown;
    }
}
