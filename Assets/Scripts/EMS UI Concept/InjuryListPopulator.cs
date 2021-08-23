using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InjuryListPopulator : MonoBehaviour
{
    EmsUiManager emsUi;
    EmsPinManager emsPin;

    public GameObject injuryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        emsUi = FindObjectOfType<EmsUiManager>();
        emsPin = FindObjectOfType<EmsPinManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < emsPin.transform.childCount; i++)
        {
            EmsPin pin = emsPin.transform.GetChild(i).GetComponent<EmsPin>();
            ListInjury(i + 1, pin.textString);
        }
    }

    public void ListInjury(int index, string text)
    {
        TextMeshProUGUI textMesh = Instantiate(injuryPrefab, transform).GetComponent<TextMeshProUGUI>();
        textMesh.text = index + ". " + text;
    }
}
