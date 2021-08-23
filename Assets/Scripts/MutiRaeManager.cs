using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MutiRaeManager : MonoBehaviour
{
    public enum Gas
    {
        O2=0, LEL=1, CO=2, H2S=3, VOC=4, GAMMA=5 
    }

    [System.Serializable]
    public class GasIndicator
    {
        enum color { GREY, GREEN, YELLOW, RED}

        public float value;
        public string valueString;
        public Text valueText;//, nameText;
        public TextMeshProUGUI pelicanValueText;
        public TextMeshProUGUI pelicanMaxValueText;
        public float greyTresh, greenThresh, yellowThresh, redThresh, maxValue = 0;
        public GameObject grey, green, yellow, red;
        public bool reversed;

        bool thresholdChanged;

        color previousColor = color.GREY;
        color currentColor;

        public void setValue(float val, float lerpPercent = 1)
        {
            value += (val - value) * lerpPercent;
            valueString = string.Format("{0:0.0}", value);

            if (valueText != null)
                valueText.text = valueString;

            if (pelicanValueText != null)
                pelicanValueText.text = valueString;


            if (value < greenThresh)
            {
                setColor(color.GREY);
            } else if (value < yellowThresh)
            {
                setColor(color.GREEN);
            } else if (value < redThresh)
            {
                setColor(color.YELLOW);
            } else 
            {
                setColor(color.RED);
            }

            thresholdChanged = (currentColor != previousColor);
            previousColor = currentColor;

            maxValue = Mathf.Max(value, maxValue);
            string maxValueString = string.Format("{0:0.0}", maxValue);
            if (pelicanMaxValueText != null)
                pelicanMaxValueText.text = maxValueString;

        }

        public float GetMaxValue()
        {
            return maxValue;
        }

        void setColor(color c)
        {
            currentColor = c;

            grey.SetActive(false);
            green.SetActive(false);
            yellow.SetActive(false);
            red.SetActive(false);

            switch (c)
            {
                case color.GREY:
                    grey.SetActive(true);
                    break;
                case color.GREEN:
                    green.SetActive(true);
                    break;
                case color.YELLOW:
                    yellow.SetActive(true);
                    break;
                case color.RED:
                    red.SetActive(true);
                    break;
                default:
                    grey.SetActive(true);
                    break;
            }
        }

        public bool shouldAlarm()
        {
            if (thresholdChanged)
            {
                if (reversed)
                {
                    if(currentColor == color.GREEN || currentColor == color.GREY)
                    {
                        return true;
                    }
                }
                else
                {
                    if(currentColor == color.RED || currentColor == color.YELLOW)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }

    public float updateInterval= .1f;
    public float incrementalPercentage = .05f;
    float timer = 0;

    public GasIndicator O2, LEL, CO, H2S, VOC, GAMMA;

    GasIndicator[] gasses;
    ProximityEmitter[] emitters;

    public AudioSource audioSource;
    public AudioClip changeSound;


    // Start is called before the first frame update
    void Start()
    {
        gasses = new GasIndicator[] { O2, LEL, CO, H2S, VOC, GAMMA };
        emitters = FindObjectsOfType<ProximityEmitter>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateInterval)
        {
            timer -= updateInterval;
            for (int i = 0; i < gasses.Length; i++)
            {
                setValue((Gas)i, getValue((Gas)i));
            }
        }
    }

    public float GetMaxValue(int index)
    {
        if (index >= 0 && index < gasses.Length)
        {
            return gasses[index].maxValue;
        }
        return -9999;
    }

    void setValue(Gas gas, float value)
    {
        GasIndicator indicator = gasses[(int)gas];
        indicator.setValue(value, incrementalPercentage);

        if (indicator.shouldAlarm())
        {
            playSound(changeSound);         
        }
    }

    float getValue(Gas type)
    {
        float total = 0;

        for (int i = 0; i < emitters.Length; i++)
        {
            if (emitters[i].type == type)
            {
                total += emitters[i].getValue(transform);
            }
        }

        return total;
    }

    void playSound(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
