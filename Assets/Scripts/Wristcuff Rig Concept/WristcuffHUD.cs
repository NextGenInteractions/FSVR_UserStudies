using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristcuffHUD : MonoBehaviour
{

    public Material litDown;
    public Material litUp;

    public Renderer[] squares;

    public Animator anim;

    private TouchPadHandler tph;

    private void Awake()
    {
        tph = GetComponent<TouchPadHandler>();
    }

    private void Update()
    {
        anim.SetBool("IsTouchingPad", tph.isTouching);
    }

    public void LightUp(int i)
    {
        //Animate Holo-HUD.
        foreach (Renderer square in squares) square.material = litDown;
        squares[i].material = litUp;

        //Change the physical hand's pose.
        anim.SetInteger("Quadrant", i);
    }
}
