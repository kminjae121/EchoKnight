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
        [SerializeField] private TextMeshProUGUI currentCostText;
        [SerializeField] private TextMeshProUGUI maxCostText;
        [SerializeField] private TurnCostGaugeManager gaugeManager;
        [SerializeField] private float gaugeTweenTime = 0.3f;

        private Tween gaugeTween;

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

            float targetFill = gaugeManager.currentGaugeValue.Value / (float)gaugeManager.maxGaugeValue;
            
            gaugeTween?.Kill();

            gaugeTween = turnCostGaugeImage
                .DOFillAmount(targetFill, gaugeTweenTime)
                .SetEase(Ease.OutCubic);
        }
    }
}