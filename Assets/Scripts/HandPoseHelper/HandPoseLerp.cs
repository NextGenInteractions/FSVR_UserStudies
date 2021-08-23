using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using YamlDotNet.Core.Tokens;

public class HandPoseLerp : MonoBehaviour
{
    [Tooltip("Hand Palm in this case")]
    public Transform handPalmRoot;
    public Transform lerpHandRoot;
    public Transform predefineHandRoot;
    public float lerpDuration = 1f;
    Dictionary<string, Transform> handTrans = new Dictionary<string, Transform>();
    Dictionary<string, Transform> lerpTrans = new Dictionary<string, Transform>();
    Dictionary<string, Transform> predefinedTrans = new Dictionary<string, Transform>();
    public bool isLerping { get => timer >= 0; }
    private bool isForward = false;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        predefinedTrans.Add(predefineHandRoot.name, predefineHandRoot);
        handTrans.Add(handPalmRoot.name, handPalmRoot);
        lerpTrans.Add(lerpHandRoot.name, lerpHandRoot);
        LoadHandTransform(lerpHandRoot, lerpTrans);
        LoadHandTransform(predefineHandRoot, predefinedTrans);
        LoadHandTransform(handPalmRoot, handTrans);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0)
        {
            if (isForward)
            {
                foreach(string key in predefinedTrans.Keys)
                {
                    if (key.Equals("L_Palm"))
                    {
                        lerpTrans[key].position = Vector3.Lerp(handTrans[key].position, predefinedTrans[key].position, (lerpDuration - timer) / lerpDuration);
                        lerpTrans[key].rotation = Quaternion.Lerp(handTrans[key].rotation, predefinedTrans[key].rotation, (lerpDuration - timer) / lerpDuration);
                    }
                    else
                    {
                        lerpTrans[key].localPosition = Vector3.Lerp(handTrans[key].localPosition, predefinedTrans[key].localPosition, (lerpDuration - timer) / lerpDuration);
                        lerpTrans[key].localRotation = Quaternion.Lerp(handTrans[key].localRotation, predefinedTrans[key].localRotation, (lerpDuration - timer) / lerpDuration);
                    }
                }
            }
            else
            {
                foreach (string key in predefinedTrans.Keys)
                {
                    if (key.Equals("L_Palm"))
                    {
                        lerpTrans[key].position = Vector3.Lerp(predefinedTrans[key].position, handTrans[key].position, (lerpDuration - timer) / lerpDuration);
                        lerpTrans[key].rotation = Quaternion.Lerp(predefinedTrans[key].rotation, handTrans[key].rotation, (lerpDuration - timer) / lerpDuration);
                    }
                    else
                    {
                        lerpTrans[key].localPosition = Vector3.Lerp(predefinedTrans[key].localPosition, handTrans[key].localPosition, (lerpDuration - timer) / lerpDuration);
                        lerpTrans[key].localRotation = Quaternion.Lerp(predefinedTrans[key].localRotation, handTrans[key].localRotation, (lerpDuration - timer) / lerpDuration);
                    }
                }
            }
            timer -= Time.deltaTime;
        }
    }

    public void PlayforwardLerp()
    {
        timer = lerpDuration;
        isForward = true;
    }

    public void PlaybackLerp()
    {
        timer = lerpDuration;
        isForward = false;
    }

    void LoadHandTransform(Transform current, Dictionary<string, Transform> dic)
    {
        for (int i = 0; i < current.childCount; ++i)
        {
            Transform next = current.GetChild(i);
            dic.Add(next.name, next);
            LoadHandTransform(next, dic);
        }
    }
}
