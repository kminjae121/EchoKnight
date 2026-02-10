using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterSkillButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private Image skillImage;
        [SerializeField] private Color equippedColor;
        [SerializeField] private Color unequippedColor;

        private SkillSO _skillInfo;
        private bool _isEquipped;
        
        public void SetSkill(SkillSO skill, bool isEquipped)
        {
            _skillInfo = skill;
            skillImage.sprite = skill.skillUIImage;
            skillNameText.text = skill.skillName;
            _isEquipped = isEquipped;

            RefreshColor();
        }
        
        public void HandleSkillButton()
        {
            if (_isEquipped)
                Bus<SkillUnequipEvent>.Raise(new SkillUnequipEvent(_skillInfo));
            else
                Bus<SkillEquipEvent>.Raise(new SkillEquipEvent(_skillInfo));

            _isEquipped = !_isEquipped;
            RefreshColor();
        }
        
        private void RefreshColor()
        {
            skillImage.color = _isEquipped ? equippedColor : unequippedColor;
        }
    }
}