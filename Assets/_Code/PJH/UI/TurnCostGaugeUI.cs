using System;
using Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class TurnCostGaugeUI : MonoBehaviour
    {
        [SerializeField] private Image turnCostGaugeImage;
        [SerializeField] private TextMeshProUGUI currentCostText;
        [SerializeField] private TextMeshProUGUI maxCostText;
        [SerializeField] private TurnCostGaugeManager gaugeManager;

        private void OnEnable()
        {
            gaugeManager.currentGaugeValue.OnValueChanged += HandleCurrentGaugeValueChanged;
        }

        private void OnDisable()
        {
            gaugeManager.currentGaugeValue.OnValueChanged -= HandleCurrentGaugeValueChanged;
        }

        private void Start()
        {
            ReflashGauge(0);
        }

        private void HandleCurrentGaugeValueChanged(int prev, int next)
        {
            ReflashGauge(next);
        }

        private void ReflashGauge(int value)
        {
            currentCostText.text = value.ToString();
            maxCostText.text = gaugeManager.maxGaugeValue.ToString();
            turnCostGaugeImage.fillAmount = gaugeManager.currentGaugeValue.Value / (float)gaugeManager.maxGaugeValue;
        }
    }
}