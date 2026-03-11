using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterSkillButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPoolable
    {
        [Header("Pooling Settings")]
        [SerializeField] private PoolingItemSO poolingType;

        [Header("UI Elements")]
        [SerializeField] private Image iconImage;

        private SkillSO _skill;
        private bool _isEquipped;
        private GondrLib.ObjectPool.Runtime.Pool _pool;

        private readonly Color _unequippedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        private readonly Color _hoverColor = new Color(0.65f, 0.65f, 0.65f, 1f);
        private readonly Color _equippedColor = Color.white;

        public PoolingItemSO PoolingType => poolingType;
        public GameObject GameObject => gameObject;

        public void SetUpPool(GondrLib.ObjectPool.Runtime.Pool pool) => _pool = pool;

        public void ResetItem()
        {
            _skill = null;
            _isEquipped = false;
        }

        public void ReturnToPool()
        {
            if (_pool != null) _pool.Push(this);
            else Destroy(gameObject);
        }

        public void SetSkill(SkillSO skill, bool isEquipped)
        {
            _skill = skill;
            iconImage.sprite = skill.skillUIImage;
            _isEquipped = isEquipped;
            
            iconImage.color = _isEquipped ? _equippedColor : _unequippedColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_skill != null) 
            {
                iconImage.color = _isEquipped ? _equippedColor : _hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_skill != null) 
            {
                iconImage.color = _isEquipped ? _equippedColor : _unequippedColor;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_skill == null) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Bus<SkillDetailSelectEvent>.Raise(new SkillDetailSelectEvent(_skill));
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Bus<SkillEquipPopupEvent>.Raise(new SkillEquipPopupEvent(_skill, _isEquipped, eventData.position));
            }
        }
    }
}