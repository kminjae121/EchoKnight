using System;
using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.UI;

public class APSlider : MonoBehaviour
{
    [SerializeField] private Slider apSlider;

    private void Start()
    {
        Bus<ApSliderEvent>.Subscribe(ApSlider);
    }

    public void ApSlider(ApSliderEvent evt)
    {
        float value = evt.value;
        
        apSlider.value = value;
    }
}
