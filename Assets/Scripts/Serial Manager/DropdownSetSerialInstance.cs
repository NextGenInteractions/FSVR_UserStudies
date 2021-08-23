using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSetSerialInstance : MonoBehaviour
{
    public SerialInterpreter interpreter;
    private Dropdown dropdown;

    private void Awake()
    {
        gameObject.AddComponent<PopulateDropdownWithSerialNames>();
        dropdown = GetComponent<Dropdown>();
    }

    public void SetSerialInstance(int toSet)
    {
        if(interpreter)
        {
            if(toSet == 0)
            {
                interpreter.SetInactive();
            }
            else
            {
                string name = dropdown.options[toSet].text;
                bool success = interpreter.SetSerialInstance(SerialManager.GetSerialInstanceByName(name));
                if (!success)
                    dropdown.SetValueWithoutNotify(0);
            }
        }
    }
}
