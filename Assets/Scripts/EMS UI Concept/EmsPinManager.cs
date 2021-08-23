using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmsPinManager : MonoBehaviour
{
    public GameObject pinPrefab;
    private EmsUiManager emsUiManager;

    public GameObject placeholderButtons;

    private void Awake()
    {
        emsUiManager = FindObjectOfType<EmsUiManager>();
        //NewPin();
    }

    public void NewPin()
    {
        Instantiate(pinPrefab, transform);
    }

    public void PlacePin()
    {
        foreach(EmsPin pin in GetComponentsInChildren<EmsPin>())
        {
            if (!pin.placed)
            {
                pin.Place();
                break;
            }
        }
    }

    public void DestroyUnplacedPin()
    {
        foreach(EmsPin pin in GetComponentsInChildren<EmsPin>())
        {
            if (!pin.placed)
            {
                Destroy(pin.gameObject);
                break;
            }
        }
    }

    public void OpenSpeechMenu()
    {
        placeholderButtons.SetActive(true);
        placeholderButtons.GetComponentInChildren<SpeechMenu>().Reset();
    }

    public void CloseSpeechMenu()
    {
        placeholderButtons.SetActive(false);
    }

    public void SetText(string toSet)
    {
        foreach (EmsPin pin in GetComponentsInChildren<EmsPin>())
        {
            if (!pin.HasText)
            {
                pin.SetText(toSet);
                break;
            }
        }
    }
}
