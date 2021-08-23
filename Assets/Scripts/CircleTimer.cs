using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleTimer : MonoBehaviour
{

    public float timerLength = 2;

    public AudioSource audio;
    public AudioClip processSound;
    public AudioClip failSound;
    public AudioClip passSound;
    public Text progressText;
    public bool useSeconds;

    bool timerEnabled;
    bool timerFinished;
    float completion;
    Image circle;
    Text text;


    private void Awake()
    {
        circle = GetComponent<Image>();
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Debug code

        /*        if (Input.GetKeyDown(KeyCode.Q))
                {
                    startTimer();
                }
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    failTimer();
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    resetTimer();
                }*/
        if (useSeconds)
        {
            progressText.text = string.Format("{0:0.00} s", (completion * timerLength));
        }
        else
        {
            progressText.text = string.Format("{0:0.}%", (completion * 100));
        }
        if (!timerFinished)
        {
            if (timerEnabled)
            {
                completion += Time.deltaTime / timerLength;
                if (completion >= 1.0f)
                {
                    timerFinished = true;
                    playSound(passSound);
                }
            }
            else
            {
                completion -= Time.deltaTime;
                if(completion <= 0)
                {
                    stopAudio();
                }
            }
        }

        completion = Mathf.Clamp(completion, 0.0f, 1.0f);
        setCircleCompletion(completion);
        
    }

    public void startTimer()
    {
        
        if (!timerFinished)
        {
            Debug.Log("Timer Started");
            timerEnabled = true;
            playSound(processSound);
        }
    }
    public void failTimer()
    {
        Debug.Log("Timer Failed");
        if (!timerFinished)
        {
            timerEnabled = false;
            playSound(failSound);
        }
    }

    public bool isTimerFinished()
    {
        //Debug.Log("Timer Finished test");
        return timerFinished;
    }


    public void resetTimer()
    {
        Debug.Log("Timer Reset");
        timerEnabled = false;
        timerFinished = false;
        completion = 0.0f;
        setCircleCompletion(0.0f);
    }

    void setCircleCompletion(float fill)
    {
        circle.fillAmount = fill;
    }

    void playSound(AudioClip sound)
    {
        if(audio != null)
        {
            audio.clip = sound;
            audio.Play();
        }
    }
    void stopAudio()
    {
        if (audio != null)
        {
            audio.Stop();
        }
        
    }

}
