using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace NextGen.VrManager.Ui.MetricManagement
{
    public class TallyWidget : MonoBehaviour
    {
        private MetricManager.Tally tally;

        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Button decrementButton;
        [SerializeField] private Button incrementButton;

        // Start is called before the first frame update
        void Start()
        {
            decrementButton.onClick.AddListener(Decrement);
            incrementButton.onClick.AddListener(Increment);
        }

        public void Init(MetricManager.Tally _tally)
        {
            tally = _tally;
            labelText.text = tally.name;
        }

        // Update is called once per frame
        void Update()
        {
            valueText.text = tally.count.ToString();
        }

        void Decrement()
        {
            MetricManager.DecrementTally(tally.name);
        }

        void Increment()
        {
            MetricManager.IncrementTally(tally.name);
        }
    }
}

