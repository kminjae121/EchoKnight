using Code.UnitSystem.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class AttackSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI explain;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject selectMark;

        private AttackUI _owner;
        private int _index;
        private SkillSO _skill;
        
        public RectTransform Rect => transform as RectTransform;
        public SkillSO Skill => _skill;

        public void Initialize(AttackUI owner, int index)
        {
            _owner = owner;
            _index = index;
        }

        public void SetSkill(SkillSO skill)
        {
            _skill = skill;

            if (skillName != null)
                skillName.text = skill.skillName;

            if (explain != null)
                explain.text = $"코스트 - {skill.SkillCost}";

            if (icon != null)
                icon.sprite = skill.skillUIImage;
        }
        
        public void SetSelected(bool value)
        {
            if (selectMark != null)
                selectMark.SetActive(value);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }
    }
}