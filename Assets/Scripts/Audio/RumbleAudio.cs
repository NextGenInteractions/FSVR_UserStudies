using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleAudio : MonoBehaviour
{
    public AudioSource source;
    public RouteInterpolation route;
    public AudioClip clip;
    public AnimationCurve curve;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (source != null && source.isPlaying && route != null)
        {
            source.volume = Mathf.Clamp(curve.Evaluate(route.CurrentSpeed / route.TopSpeed), 0, 1);
        }
    }

    public void PlayClip ()
    {
        PlayClip(clip);
    }
    public void PlayClip(AudioClip audioClip)
    {
        if (source == null)
            return;
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = audioClip;
        source.Play();
    }

    public void StopClip ()
    {
        if (source != null && source.isPlaying)
            source.Stop();
    }
}
