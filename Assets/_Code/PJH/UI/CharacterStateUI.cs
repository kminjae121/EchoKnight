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

        private UnitState _unit;
        private Tween _healthBarTween;
        
        public void SetUnit(UnitState unit)
        {
            _unit = unit;
        }
        
        private void RefreshHealthBar()
        {
            //float fillValue = _unit.CurrentHp / _unit.Data
            
            _healthBarTween?.Kill();
            //_healthBarTween = healthBar
            //    .DOFillAmount()
        }
    }
}