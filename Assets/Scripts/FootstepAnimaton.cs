using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepAnimaton : MonoBehaviour
{

    public Transform destination;
    public Transform start;

    public GameObject LeftFootPrefab;
    public GameObject RightFootPrefab;


    public float switchInterval;
    float timer;

    public float stepLength;
    public int maxSteps;

    int currentStep;


    GameObject[] steps;

    // Start is called before the first frame update
    void Start()
    {
        instantiateSteps();
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = destination.position;
        transform.LookAt(start.position);

        maxSteps = (int) (Vector3.Distance(destination.position, start.position) / stepLength);

        timer += Time.deltaTime;
        if (timer > switchInterval)
        {
            timer = 0;

            for (int i = 0; i < steps.Length; i++)
            {
                steps[i].SetActive(false);
            }

            if(currentStep <= 0)
            {
                currentStep = maxSteps;
            }
            currentStep--;

            steps[currentStep].SetActive(true);
        }

    }

    void instantiateSteps()
    {
        steps = new GameObject[maxSteps];

        for (int i = 0; i < steps.Length; i++)
        {
            if(i % 2 == 0)
            {
                steps[i] = Instantiate(LeftFootPrefab, transform.forward * stepLength * i, Quaternion.identity);
            }
            else
            {
                steps[i] = Instantiate(RightFootPrefab, transform.forward * stepLength * i, Quaternion.identity);
            }

            steps[i].transform.parent = this.transform;

        }
    }
    

}
