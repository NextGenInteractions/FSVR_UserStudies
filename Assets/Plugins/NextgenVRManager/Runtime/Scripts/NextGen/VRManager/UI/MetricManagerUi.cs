using NextGen.VrManager.Ui.MetricManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetricManagerUi : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Button playPauseButton;
    [SerializeField] private Button eventButton;
    [SerializeField] private TextMeshProUGUI sessionDurationText;
    [SerializeField] private CanvasGroup eventTextFieldCanvasGroup;
    [SerializeField] private TMP_InputField eventTextField;
    [SerializeField] private TMP_InputField sessionNameField;
    [SerializeField] private Transform widgetMount;

    [Header("Icon Assets")]
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseIcon;

    [Header("Widget Prefabs")]
    [SerializeField] private GameObject tallyWidgetPrefab;
    [SerializeField] private GameObject timerWidgetPrefab;

    [Header("Debug Buttons")]
    [SerializeField] private bool clickToCreateTally;
    [SerializeField] private bool clickToCreateTimer;
    private int widgetCount = 0;

    //---

    private bool eventState = false;
    private float declaredEventTime;

    //---

    public static string sessionNameInField;

    void OnEnable()
    {
        MetricManager.onTallyCreated += CreateTallyWidget;
        MetricManager.onTimerCreated += CreateTimerWidget;
        MetricManager.onSessionStopped += ClearWidgets;
    }

    private void OnDisable()
    {
        MetricManager.onTallyCreated -= CreateTallyWidget;
        MetricManager.onTimerCreated -= CreateTimerWidget;
        MetricManager.onSessionStopped -= ClearWidgets;
    }

    // Start is called before the first frame update
    void Start()
    {
        playPauseButton.onClick.AddListener(ToggleIsLive);
        eventButton.onClick.AddListener(EventButton);
        eventTextField.onEndEdit.AddListener(EventTextFieldOnEndEdit);
    }

    // Update is called once per frame
    void Update()
    {
        sessionDurationText.gameObject.SetActive(MetricManager.isLive);
        sessionNameField.gameObject.SetActive(!MetricManager.isLive);

        if (MetricManager.isLive)
            sessionDurationText.text = MetricManager.TimeToMSD(MetricManager.sessionDuration);
        else
            sessionNameInField = sessionNameField.text;

        if(MetricManager.isLive)
        {
            if (clickToCreateTally)
            {
                MetricManager.CreateTally($"Cool Tally {widgetCount++}", 0);
                clickToCreateTally = false;
            }
            if (clickToCreateTimer)
            {
                MetricManager.CreateTimer($"Cool Timer {widgetCount++}");
                clickToCreateTimer = false;
            }
        }
    }

    void ToggleIsLive()
    {
        MetricManager.ToggleIsLive(sessionNameField.text);

        playPauseButton.transform.GetChild(0).GetComponent<Image>().sprite = MetricManager.isLive ? pauseIcon : playIcon;
    }

    void EventButton()
    {
        if (!eventState)
            DeclareEvent();
        else
            SubmitEvent();
    }

    void EventTextFieldOnEndEdit(string body)
    {
        SubmitEvent();
    }

    void DeclareEvent()
    {
        declaredEventTime = MetricManager.sessionDuration;
        eventTextFieldCanvasGroup.interactable = true;
        eventTextField.Select();

        eventState = true;
    }

    void SubmitEvent()
    {
        MetricManager.LogEvent(declaredEventTime, "UI", eventTextField.text);

        eventTextFieldCanvasGroup.interactable = false;
        eventTextField.text = "";

        eventState = false;
    }

    void CreateTallyWidget(MetricManager.Tally tally)
    {
        Instantiate(tallyWidgetPrefab, widgetMount).GetComponent<TallyWidget>().Init(tally);
    }

    void CreateTimerWidget(MetricManager.Timer timer)
    {
        Instantiate(timerWidgetPrefab, widgetMount).GetComponent<TimerWidget>().Init(timer);
    }

    void ClearWidgets()
    {
        for(int i = 0; i < widgetMount.childCount; i++)
        {
            Destroy(widgetMount.GetChild(i).gameObject);
        }
    }
}
