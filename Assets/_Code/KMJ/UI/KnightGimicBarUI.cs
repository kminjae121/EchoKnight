using System;
using Code.Core.Events.Bus;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class KnightGimicBarUI : MonoBehaviour
    {
        [SerializeField] private Image gaugeImg;
        private void Awake()
        {
            Bus<KnightGimicBarEvent>.Subscribe(SetKnightGimicBar);
        }

        private void OnDisable()
        {
            Bus<KnightGimicBarEvent>.Unsubscribe(SetKnightGimicBar);
            
        }

        private void SetKnightGimicBar(KnightGimicBarEvent evt)
        {
            gaugeImg.DOFillAmount(evt.value / 10,0.5f);
            float t = Mathf.Clamp01(evt.value / 10f);         
            float v = Mathf.Lerp(108f / 255f, 1f, t);         
            Color color = new Color(v, v, v, 1f);             
            gaugeImg.DOColor(color, 0.5f);        
        }
    }
}