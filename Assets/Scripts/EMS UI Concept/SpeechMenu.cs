using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

public class SpeechMenu : MonoBehaviour
{
    public TextMeshProUGUI instructions;

    public Mode mode = Mode.None;

    public int defaultMic;
    public string label;

    public GameObject recordingScreen;
    public GameObject continueButton;

    private GCSpeechRecognition _speechRecognition;

    public enum Mode
    {
        None,
        Record,
        Continue
    };

    private EmsUiManager ems;

    public bool steamVrConfirm;

    private void Start()
    {
        ems = FindObjectOfType<EmsUiManager>();

        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
        _speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;

        _speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
        _speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
        _speechRecognition.RecordFailedEvent += RecordFailedEventHandler;

        _speechRecognition.BeginTalkigEvent += BeginTalkigEventHandler;
        _speechRecognition.EndTalkigEvent += EndTalkigEventHandler;

        int i = 0;
        foreach (string micName in _speechRecognition.GetMicrophoneDevices())
        {
            Debug.Log(string.Format("Mic found: [{0}] {1}", i++, micName));
        }

        _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[defaultMic]);
    }

    public void Reset()
    {
        mode = Mode.Record;
        label = "";
    }

    // Update is called once per frame
    void Update()
    {
        string partA = label == "" ? "" : "Label: " + label; 
        string partB = "";

        switch(mode)
        {
            case Mode.None:
                partB = "Select Record.";
                break;

            case Mode.Record:
                if(!_speechRecognition.IsRecording && label == "")
                {
                    partB = "Press Confirm to record a label for this pin.";
                    
                }
                else if(_speechRecognition.IsRecording)
                {
                    partB = "Currently recording a new label...";
                }
                else
                {
                    partB = "Press Confirm to record another label instead.";
                }

                if(!_speechRecognition.IsRecording)
                {
                    if (Input.GetKeyDown(KeyCode.Return) || steamVrConfirm)
                    {
                        _speechRecognition.StartRecord(true);
                        recordingScreen.GetComponentInChildren<TextMeshProUGUI>(true).text = "Start speaking.";
                    }
                }
                
                break;

            case Mode.Continue:
                partB = "Press Confirm to set this as the label.";
                if (Input.GetKeyDown(KeyCode.Return) || steamVrConfirm)
                {
                    FindObjectOfType<EmsUiManager>().SetInjury(label);
                }
                break;
        }

        instructions.text = partA + "\n" + partB;
        recordingScreen.SetActive(_speechRecognition.IsRecording);
        continueButton.SetActive(label != "");

        steamVrConfirm = false;
    }

    public void SetMode(int modeToSet)
    {
        if(!_speechRecognition.IsRecording)
            mode = (Mode)modeToSet;
    }

    private void StartedRecordEventHandler()
    {
    }

    private void BeginTalkigEventHandler()
    {
        recordingScreen.GetComponentInChildren<TextMeshProUGUI>(true).text = "Recording...";
    }

    private void EndTalkigEventHandler(AudioClip clip, float[] raw)
    {
        RecognitionConfig config = RecognitionConfig.GetDefault();
        config.languageCode = Enumerators.LanguageCode.en_US.Parse();
        config.audioChannelCount = clip.channels;
        // configure other parameters of the config if need

        GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
        {
            audio = new RecognitionAudioContent()
            {
                content = raw.ToBase64()
            },
            //audio = new RecognitionAudioUri() // for Google Cloud Storage object
            //{
            //	uri = "gs://bucketName/object_name"
            //},
            config = config
        };

        _speechRecognition.Recognize(recognitionRequest); FinishedRecordEventHandler(clip, raw);
        recordingScreen.GetComponentInChildren<TextMeshProUGUI>(true).text = "Processing...";
    }

    private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
    {
    }

    private void RecordFailedEventHandler()
    {
        Debug.Log("Record failed.");
    }

    private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
    {
        Debug.Log("Number of words: " + recognitionResponse.results.Length);
        foreach (SpeechRecognitionResult result in recognitionResponse.results)
        {
            int i = 0;
            foreach (SpeechRecognitionAlternative alternative in result.alternatives)
            {
                Debug.Log(string.Format("Alternative {0}: ({1}): {2}", i++, alternative.confidence, alternative.transcript));
            }

            string utterance = result.alternatives[0].transcript;
            Debug.Log("Utterance: " + utterance);
            //if (keywordListener != null) keywordListener.Check(utterance.ToLower());
            //if (keywordListener != null) keywordListener.CheckAlternatives(result.alternatives);
            label = utterance;


            //if (line.Contains("pop") && line.Contains("trunk")) Debug.Log("Walla walla washington");
        }
        _speechRecognition.StopRecord();
    }
    private void RecognizeFailedEventHandler(string error)
    {
        Debug.Log("Failed to recognize: " + error);
    }
}
