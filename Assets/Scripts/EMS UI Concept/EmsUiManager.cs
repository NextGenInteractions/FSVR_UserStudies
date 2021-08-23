using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmsUiManager : MonoBehaviour
{
    public Screen screen = Screen.SexSelection;
    public enum Screen
    {
        SexSelection,
        RegionSelection,
        Placement,
        InjurySelection
    };

    public Sex sex = Sex.Unknown;
    public enum Sex
    {
        Unknown,
        Male,
        Female
    };

    public Region region = Region.None;
    public enum Region
    {
        None,
        Head,
        Torso,
        RightArm,
        LeftArm,
        RightLeg,
        LeftLeg,
    };

    [Header("Sex Selection")]
    public GameObject sexSelection;
    public TextMeshProUGUI instructions;
    public Image maleButton;
    private TextMeshProUGUI maleText;
    public Image femaleButton;
    private TextMeshProUGUI femaleText;

    [Header("Male Region Selection")]
    public GameObject maleRegionSelection;
    public Image[] maleRegions;
    public GameObject[] maleRegionButtons;
    private Color pink = new Color32(255, 192, 192, 255);
    public FullBodyTransformer fullBodyTransformer;
    private EmsPinManager emsPinManager;
    public InjuryListPopulator injuryListPopulator;

    [Header("Injury Selection")]
    public int injurySelected;

    [HideInInspector] public bool steamVrConfirm = false;
    [HideInInspector] public bool steamVrBack = false;

    private void Awake()
    {
        maleText = maleButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        femaleText = femaleButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        emsPinManager = GetComponentInChildren<EmsPinManager>(true);
    }

    private void Update()
    {
        switch (screen)
        {
            case Screen.SexSelection:
                switch(sex)
                {
                    case Sex.Unknown:
                        instructions.text = "Select the sex of the patient, then press Confirm to continue.";
                        maleText.color = Color.black;
                        femaleText.color = Color.black;
                        break;

                    case Sex.Male:
                        instructions.text = "Select the sex of the patient, then press Confirm to continue.\nCurrently Selected: Male";
                        maleText.color = Color.white;
                        femaleText.color = Color.black;
                        break;

                    case Sex.Female:
                        instructions.text = "Select the sex of the patient, then press Confirm to continue.\nCurrently Selected: Female";
                        maleText.color = Color.black;
                        femaleText.color = Color.white;
                        break;
                }
                if(Input.GetKeyDown(KeyCode.Return) || steamVrConfirm)
                {
                    if(sex == Sex.Male)
                    {
                        screen = Screen.RegionSelection;
                        sexSelection.SetActive(false);
                        maleRegionSelection.SetActive(true);
                        injuryListPopulator.Refresh();
                        Debug.Log("Moving to region selection screen...");
                    }
                }
                break;

            case Screen.RegionSelection:
                if(region != Region.None)
                {
                    for (int i = 1; i < maleRegions.Length; i++)
                    {
                        maleRegions[i].color = i == (int)region ? pink : Color.white;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Return) || steamVrConfirm)
                {
                    if (region != Region.None)
                    {
                        fullBodyTransformer.Set((int)region);
                        SetMaleRegionButtonsActive(false);
                        screen = Screen.Placement;
                        emsPinManager.NewPin();
                        Debug.Log("Moving to pin placement screen...");
                    }
                }
                break;

            case Screen.Placement:
                for (int i = 1; i < maleRegions.Length; i++)
                {
                    maleRegions[i].color = Color.white;
                }

                if (Input.GetKeyDown(KeyCode.Backspace) || steamVrBack)
                {
                    fullBodyTransformer.Set(0);
                    SetMaleRegionButtonsActive(true);
                    screen = Screen.RegionSelection;
                    emsPinManager.DestroyUnplacedPin();
                }
                if (Input.GetKeyDown(KeyCode.Return) || steamVrConfirm)
                {
                    emsPinManager.PlacePin();
                    emsPinManager.OpenSpeechMenu();
                    screen = Screen.InjurySelection;
                    Debug.Log("Moving to speech menu...");
                }
                break;
        }

        if (steamVrConfirm)
            steamVrConfirm = false;
        if (steamVrBack)
            steamVrBack = false;
    }

    public void SetSex(bool isMale)
    {
        sex = isMale ? Sex.Male : Sex.Female;
    }

    public void SetRegion(int index)
    {
        region = (Region)index;
    }

    public void SetInjury(string label)
    {
        emsPinManager.SetText(label);
        emsPinManager.CloseSpeechMenu();

        fullBodyTransformer.Set(0);
        SetMaleRegionButtonsActive(true);
        screen = Screen.RegionSelection;
        region = Region.None;
        injuryListPopulator.Refresh();
    }

    public void SetMaleRegionButtonsActive(bool enable)
    {
        foreach(GameObject button in maleRegionButtons)
        {
            button.SetActive(enable);
        }
    }

    public void SteamVrConfirm()
    {
        steamVrConfirm = true;
        if (GetComponentInChildren<SpeechMenu>())
            GetComponentInChildren<SpeechMenu>().steamVrConfirm = true;
    }

    public void SteamVrBack()
    {
        steamVrBack = true;
    }
}
