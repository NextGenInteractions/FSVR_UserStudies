using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NextGen.VrManager.Ui.MetricManagement
{
    public class TimerWidget : MonoBehaviour
    {
        private MetricManager.Timer timer;

        [SerializeField] private Sprite playSprite;
        [SerializeField] private Sprite pauseSprite;

        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI lapTimeText;
        [SerializeField] private Button playPauseButton;
        [SerializeField] private Button lapButton;
        [SerializeField] private Image playPauseIcon;


        // Start is called before the first frame update
        void Start()
        {
            playPauseButton.onClick.AddListener(PlayPause);
            lapButton.onClick.AddListener(Lap);
        }

        public void Init(MetricManager.Timer _timer)
        {
            timer = _timer;
            labelText.text = timer.name;
        }

        // Update is called once per frame
        void Update()
        {
            timeText.text = MetricManager.TimeToMSD(timer.time);
            lapTimeText.text = MetricManager.TimeToMSD(timer.time);

            playPauseIcon.sprite = timer.active ? pauseSprite : playSprite;
        }

        void PlayPause()
        {
            if (timer.active)
                MetricManager.StopTimer(timer.name);
            else
                MetricManager.StartTimer(timer.name);
        }

        void Lap()
        {

        }
    }
}