using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionLerp : MonoBehaviour
{
    [SerializeField]
    private bool runForeverOnStart;

    public Color emissionColor = Color.white;
    [Range(0, 1)]
    public float minIntensity;
    [Range(0, 1)]
    public float maxIntensity = 1;

    public Renderer targetRenderer;
    public int rendererMaterialIndex = 0;
    public Material customMaterial;

    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public float fadeInStayTime = .25f;
    public float fadeOutStayTime = .25f;

    [SerializeField]
    private FadeState currentState;
    [SerializeField]
    private float timeInCurrentState;

    private Coroutine currentLoop;

    private Material originalMaterial;

    private bool stopLoop = false;

    private void Start()
    {
        if(targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if(runForeverOnStart)
        {
            StartEmissionLoop(0);
        }
    }

    public void StartEmissionLoop(int numLoops = 0)
    {
        if(customMaterial)
        {
            originalMaterial = targetRenderer.materials[rendererMaterialIndex];
            targetRenderer.materials[rendererMaterialIndex] = customMaterial;
        }

        if(currentLoop != null)
        {
            StopEmissionLoopImmediate();
        }
        currentLoop = StartCoroutine(EmissionLoop(numLoops));
    }

    public void StopEmissionLoopImmediate()
    {
        if(currentLoop != null)
        {
            StopCoroutine(currentLoop);
            currentLoop = null;

            targetRenderer.materials[rendererMaterialIndex].SetColor("_EmissionColor", emissionColor * minIntensity);

            if (originalMaterial)
            {
                targetRenderer.materials[rendererMaterialIndex] = originalMaterial;
            }
        }
    }

    public void StopEmissionLoop()
    {
        if(currentLoop != null)
            stopLoop = true;
    }

    private IEnumerator EmissionLoop(int numLoops)
    {
        targetRenderer.materials[rendererMaterialIndex].EnableKeyword("_EMISSION");

        int completedLoops = 0;
        currentState = FadeState.FadingIn;
        while (true)
        {          
            float timeToStayInState = GetTimeToStayInState(currentState);

            timeInCurrentState += Time.deltaTime;

            if(timeInCurrentState > timeToStayInState)
            {
                currentState = GetNextState(currentState);
                if (currentState == FadeState.FadingIn)
                {
                    completedLoops++;
                    //if numLoops is less than 1, loop until told to stop
                    if ((numLoops >= 1 && completedLoops >= numLoops) || stopLoop)
                    {
                        stopLoop = false;
                        break;
                    }
                }
                timeInCurrentState = 0;
                //get time to stay in new state
                timeToStayInState = GetTimeToStayInState(currentState);
            }

            float currentEmissionIntensity = 0;

            if(currentState == FadeState.FadingIn)
                currentEmissionIntensity = Mathf.Lerp(minIntensity, maxIntensity, timeInCurrentState / timeToStayInState);
            if (currentState == FadeState.FadingOut)
                currentEmissionIntensity = Mathf.Lerp(minIntensity, maxIntensity, 1 - (timeInCurrentState / timeToStayInState));
            if (currentState == FadeState.In)
                currentEmissionIntensity = maxIntensity;
            if (currentState == FadeState.Out)
                currentEmissionIntensity = minIntensity;

            targetRenderer.materials[rendererMaterialIndex].SetColor("_EmissionColor", emissionColor * currentEmissionIntensity);

            yield return null;
        }
        StopEmissionLoopImmediate();
    }

    private float GetTimeToStayInState(FadeState state)
    {
        switch (state)
        {
            case FadeState.FadingIn:
                return fadeInTime;
            case FadeState.FadingOut:
                return fadeOutTime;
            case FadeState.In:
                return fadeInStayTime;
            case FadeState.Out:
            default:
                return fadeOutStayTime;
        }
    }

    private FadeState GetNextState(FadeState state)
    {
        switch (state)
        {
            case FadeState.FadingIn:
                return FadeState.In;
            case FadeState.FadingOut:
                return FadeState.Out;
            case FadeState.In:
                return FadeState.FadingOut;
            case FadeState.Out:
            default:
                return FadeState.FadingIn;
        }
    }

    enum FadeState
    {
        FadingIn,
        In,
        FadingOut,
        Out
    }
}
