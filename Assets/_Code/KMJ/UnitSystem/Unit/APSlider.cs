using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.UI;

public class APSlider : MonoBehaviour
{
    [SerializeField] private Slider apSlider;

    private float _targetValue;

    private void Start()
    {
        Bus<ActionGaugeEvent>.Subscribe(ApSlider);
    }

    private void OnDisable()
    {
        Bus<ActionGaugeEvent>.Unsubscribe(ApSlider);
    }

    private void FixedUpdate()
    {
        apSlider.value = Mathf.Lerp(apSlider.value, _targetValue, Time.fixedDeltaTime * 2);
    }

    public void ApSlider(ActionGaugeEvent evt)
    {
        float value = evt.Value;
        
        _targetValue = value;
    }
}
