using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterSkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        [SerializeField] private Image skillImage;
        
        [Header("Colors")]
        [SerializeField] private Color equippedColor;
        [SerializeField] private Color unequippedColor;

        private SkillSO _skillInfo;
        private bool _isEquipped;

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

        public void SetSkill(SkillSO skill, bool isEquipped)
        {
            _skillInfo = skill;
            skillImage.sprite = skill.skillUIImage;
            _isEquipped = isEquipped;

            RefreshColor();
        }
        
        public void HandleSkillButton()
        {
            if (_isEquipped)
                Bus<SkillUnequipEvent>.Raise(new SkillUnequipEvent(_skillInfo));
            else
                Bus<SkillEquipEvent>.Raise(new SkillEquipEvent(_skillInfo));
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
            Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(_skillInfo, (RectTransform)transform));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
        }
    }
}