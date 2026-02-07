using Code.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class TurnCostGaugeUI : MonoBehaviour
    {
        [SerializeField] private Image turnCostGaugeImage;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TurnCostGaugeManager gaugeManager;
        [SerializeField] private float gaugeTweenTime = 0.3f;

        private Tween _gaugeTween;

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
            RefreshGauge(100);
        }

        private void HandleCurrentGaugeValueChanged(int prev, int next)
        {
            RefreshGauge(next);
        }

        private void RefreshGauge(int value)
        {
            costText.text = $"{value} / {gaugeManager.maxGaugeValue}";

            float targetFill = gaugeManager.currentGaugeValue.Value / (float)gaugeManager.maxGaugeValue;
            
            _gaugeTween?.Kill();
            _gaugeTween = turnCostGaugeImage
                .DOFillAmount(targetFill, gaugeTweenTime)
                .SetEase(Ease.OutCubic);
        }
    }
}