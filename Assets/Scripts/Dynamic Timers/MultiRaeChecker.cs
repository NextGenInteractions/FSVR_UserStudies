using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRaeChecker : MonoBehaviour
{
    [SerializeField] private float checkRadius = 0.125f;
    [SerializeField] private bool lastMultiRaePresent = false;
    [SerializeField] private bool multiRaePresent = false;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip enterSfx;
    [SerializeField] private AudioClip timerSfx;

    [Header("Timer Range")]
    [SerializeField] private float timerMin;
    [SerializeField] private float timerMax;
    [SerializeField] private float timerThreshold;
    private bool timerPassedYet;

    private bool insideRadius;
    [SerializeField] private float timeElapsedSinceEnter;

    private AudioSource aud;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        multiRaePresent = false;

        foreach (Collider collider in Physics.OverlapSphere(transform.position, checkRadius))
        {
            if(collider.GetComponent<IsAMultiRae>() != null)
            {
                multiRaePresent = true;
                break;
            }
        }

        if(!lastMultiRaePresent && multiRaePresent)
            MultiRaeEnter();

        if (lastMultiRaePresent && !multiRaePresent)
            MultiRaeExit();

        lastMultiRaePresent = multiRaePresent;

        if (insideRadius)
        {
            timeElapsedSinceEnter += Time.deltaTime;
            if(!timerPassedYet)
            {
                if (timeElapsedSinceEnter >= timerThreshold)
                {
                    aud.PlayOneShot(timerSfx);
                    timerPassedYet = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

    private void OnDrawGizmosSelected()
    {
        
    }

    private void MultiRaeEnter()
    {
        GetComponent<DynamicTimer>().StartTimer();

        insideRadius = true;
        timeElapsedSinceEnter = 0;

        timerThreshold = Mathf.RoundToInt(Random.Range(timerMin, timerMax));
        timerPassedYet = false;

        aud.PlayOneShot(enterSfx);
    }

    private void MultiRaeExit()
    {
        GetComponent<DynamicTimer>().StopTimer();

        if (insideRadius && !timerPassedYet)
            aud.PlayOneShot(timerSfx);
        insideRadius = false;
    }
}
