using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashAnimation : MonoBehaviour
{
    private Image img;
    private float age = 0;
    public float lifespan = 1;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    private void Update()
    {
        age += Time.deltaTime;

        if (age > lifespan)
            Destroy(gameObject);
        else
        {
            float value = age / lifespan;
            img.color = new Color(1, 1, 1, 1 - value);
        }
    }
}
