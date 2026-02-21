using System;
using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.UI;

public class APSlider : MonoBehaviour
{
    [SerializeField] private Slider apSlider;

    private float targetValue;
    

    private void Start()
    {
        Bus<ApSliderEvent>.Subscribe(ApSlider);
    }

    private void OnDisable()
    {
        
        Bus<ApSliderEvent>.Unsubscribe(ApSlider);
    }

    private void FixedUpdate()
    {
        apSlider.value = Mathf.Lerp(apSlider.value, targetValue, Time.fixedDeltaTime * 2);
    }

    public void ApSlider(ApSliderEvent evt)
    {
        float value = evt.value;
        
        targetValue = value;
    }
}
