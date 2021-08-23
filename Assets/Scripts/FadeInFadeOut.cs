using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour
{
    public enum Type
    {
        In,
        Out
    }
    public Type type = Type.In;
    public float timeToComplete = 1;
    Material mat;
    public bool running;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;
            Color color = mat.GetColor("_BaseColor");
            color.a = (type == Type.In) ? timer / timeToComplete : 1 - (timer / timeToComplete);
            mat.SetColor("_BaseColor", color);
            if (timer >= timeToComplete)
            {
                running = false;
                timer = 0;
            }
        } 
    }

    public void StartFade (Type fadeType = Type.Out)
    {
        type = fadeType;
        running = true;
        timer = 0;
    }

    public void StopFade ()
    {
        running = false;
    }
}
