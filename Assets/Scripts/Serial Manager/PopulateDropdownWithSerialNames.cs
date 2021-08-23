using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateDropdownWithSerialNames : MonoBehaviour
{
    Dropdown dropdown;

    public bool clickToRefresh = false;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        Populate();
    }

    private void Update()
    {
        if(clickToRefresh)
        {
            clickToRefresh = false;
            Populate();
        }
    }

    void Populate()
    {
        dropdown.ClearOptions();

        List<string> label = new List<string>();
        label.Add("Inactive");
        dropdown.AddOptions(label);
        dropdown.AddOptions(new List<string>(SerialManager.GetNames()));
    }
}
