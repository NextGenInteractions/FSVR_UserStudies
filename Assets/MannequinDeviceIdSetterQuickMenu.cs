using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinDeviceIdSetterQuickMenu : MonoBehaviour
{
    public MannequinDeviceIdSetter setter;

    public int headSet = -1;
    public int pelvisSet = -1;
    public int leftArmSet = -1;
    public int rightArmSet = -1;
    public int leftLegSet = -1;
    public int rightLegSet = -1;

    // Start is called before the first frame update
    void Start()
    {
        setter = GetComponent<MannequinDeviceIdSetter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(headSet != -1)
        {
            setter.SetHeadDeviceId(headSet);
            headSet = -1;
        }

        if (pelvisSet != -1)
        {
            setter.SetPelvisDeviceId(pelvisSet);
            pelvisSet = -1;
        }

        if (leftArmSet != -1)
        {
            setter.SetLeftArmDeviceId(leftArmSet);
            leftArmSet = -1;
        }

        if (rightArmSet != -1)
        {
            setter.SetRightArmDeviceId(rightArmSet);
            rightArmSet = -1;
        }

        if (leftLegSet != -1)
        {
            setter.SetLeftLegDeviceId(leftLegSet);
            leftLegSet = -1;
        }

        if (rightLegSet != -1)
        {
            setter.SetRightLegDeviceId(rightLegSet);
            rightLegSet = -1;
        }
    }
}
