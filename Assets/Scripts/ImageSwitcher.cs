//Image switcher randomly enables one of a set of objects.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSwitcher : MonoBehaviour
{

    public GameObject[] objects;

           // public float interval = .5f;


    public bool isDemo = true;
    public float demoInteral = .5f;
    public float trainingInterval = 1.0f;

    float timer;
    int index;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float interval = trainingInterval;

        if (isDemo)
        {
            interval = demoInteral;
        }

        if(timer > interval)
        {
            timer = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }

            //int randomindex =  Random.Range(0, objects.Length);

            index++;
            index %= objects.Length;

            objects[index].SetActive(true);

        }
        
    }
}
