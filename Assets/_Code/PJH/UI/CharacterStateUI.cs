using Code.UnitSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterStateUI : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private Image healthBar;
        [SerializeField] private float tweenTime = 0.3f;

        private UnitState _unit;
        private Tween _healthBarTween;

        private void OnDestroy()
        {
            if (_unit != null)
                _unit.CurrentHp.OnValueChanged -= RefreshHealthBar;
        }

        public void SetUnit(UnitState unit)
        {
            _unit = unit;
            _unit.CurrentHp.OnValueChanged += RefreshHealthBar;
            
            characterImage.sprite = _unit.Data.UnitImage;
        }

        private void RefreshHealthBar(float prev, float next)
        {
            float fillValue = next / _unit.Data.Maxhealth;

            _healthBarTween?.Kill();
            _healthBarTween = healthBar
                .DOFillAmount(fillValue, tweenTime)
                .SetEase(Ease.OutCubic);
        }
    }
}