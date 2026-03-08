using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterSkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPoolable
    {
        [Header("Pooling Settings")]
        [SerializeField] private PoolingItemSO poolingType;

        [Header("UI Elements")]
        [SerializeField] private Image skillImage;
        
        [Header("Colors")]
        [SerializeField] private Color equippedColor;
        [SerializeField] private Color unequippedColor;

        private SkillSO _skillInfo;
        private bool _isEquipped;
        private GondrLib.ObjectPool.Runtime.Pool _pool;

        public PoolingItemSO PoolingType => poolingType;
        public GameObject GameObject => gameObject;

        private void Awake()
        {
            Bus<SkillEquippedEvent>.Subscribe(HandleSkillEquipped);
            Bus<SkillUnequippedEvent>.Subscribe(HandleSkillUnequipped);
        }

        private void OnDestroy()
        {
            Bus<SkillEquippedEvent>.Unsubscribe(HandleSkillEquipped);
            Bus<SkillUnequippedEvent>.Unsubscribe(HandleSkillUnequipped);
        }

        public void SetUpPool(GondrLib.ObjectPool.Runtime.Pool pool)
        {
            _pool = pool;
        }

        public void ResetItem()
        {
            _skillInfo = null;
            _isEquipped = false;
            skillImage.color = unequippedColor;
        }

        public void ReturnToPool()
        {
            if (_pool != null)
                _pool.Push(this);
            else
                Destroy(gameObject);
        }

        public void SetSkill(SkillSO skill, bool isEquipped)
        {
            _skillInfo = skill;
            skillImage.sprite = skill.skillUIImage;
            _isEquipped = isEquipped;

            RefreshColor();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_skillInfo == null)
                return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Bus<SkillDetailSelectEvent>.Raise(new SkillDetailSelectEvent(_skillInfo));
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Bus<SkillEquipPopupEvent>.Raise(new SkillEquipPopupEvent(_skillInfo, _isEquipped, eventData.position));
            }
        }
        
        private void HandleSkillEquipped(SkillEquippedEvent evt)
        {
            if (evt.Skill != _skillInfo)
                return;

            _isEquipped = true;
            RefreshColor();
        }
        
        private void HandleSkillUnequipped(SkillUnequippedEvent evt)
        {
            if (evt.Skill != _skillInfo)
                return;

            _isEquipped = false;
            RefreshColor();
        }
        
        private void RefreshColor()
        {
            skillImage.color = _isEquipped ? equippedColor : unequippedColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_skillInfo != null)
                Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(_skillInfo, (RectTransform)transform));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
        }
    }
}