using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public Light LightToChange = null;
    public float TimeToChange = 0;
    public float SetToAmount = 1.0f;
    public bool increase = false;

    private void Awake()
    {
    }
    private void Update()
    {
        if (increase && LightToChange.intensity > SetToAmount)
        {
            this.enabled = false;
            return;
        }
        if (!increase && LightToChange.intensity < SetToAmount)
        {
            this.enabled = false;
            return;
        }

        ChangeValue();
    }

    private void Begin()
    {
        this.enabled = true;
    }

    private void ChangeValue()
    {
        if(increase)
        {
            LightToChange.intensity += Time.deltaTime * TimeToChange;
        }
        else
        {
            LightToChange.intensity -= Time.deltaTime * TimeToChange;
        }

    }
}