using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicNumpad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
        }
    }
}
