using System.Linq;
using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class CharacterSkillInfoUI : Panel
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI skillCostText;
        [SerializeField] private Transform skillTrm;

        [Header("Prefabs")]
        [SerializeField] private CharacterSkillButton skillPrefab;

        private UnitSO _unit;

        public override void Awake()
        {
            base.Awake();
            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            Bus<SkillEquipEvent>.Subscribe(SkillEquip);
            Bus<SkillUnequipEvent>.Subscribe(SkillUnequip);
        }

        private void OnDestroy()
        {
            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
            Bus<SkillEquipEvent>.Unsubscribe(SkillEquip);
            Bus<SkillUnequipEvent>.Unsubscribe(SkillUnequip);
        }
        
        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit.Data;
            RefreshUI();
        }
        
        private void RefreshUI()
        {
            foreach (Transform child in skillTrm)
                Destroy(child.gameObject);

            foreach (var skill in _unit.OwnSkillStorage.skills)
            {
                var skillButton = Instantiate(skillPrefab, skillTrm);
                skillButton.SetSkill(skill, _unit.SkillStorage.skills.Contains(skill));
            }
            
            skillCostText.text = $"{GetCurrentCost()} / {_unit.Cost}";
        }
        
        private void SkillEquip(SkillEquipEvent evt)
        {
            if (_unit.SkillStorage.skills.Contains(evt.Skill))
                return;
            
            if (GetCurrentCost() + evt.Skill.SkillCost > _unit.Cost)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent("코스트를 초과하여 스킬을 장착할 수 없습니다."));
                return;
            }
            
            _unit.SkillStorage.skills.Add(evt.Skill);
            Bus<SkillEquippedEvent>.Raise(new SkillEquippedEvent(evt.Skill));
            
            RefreshUI();
        }

        private void SkillUnequip(SkillUnequipEvent evt)
        {
            if (_unit.SkillStorage.skills.Remove(evt.Skill))
            {
                Bus<SkillUnequippedEvent>.Raise(new SkillUnequippedEvent(evt.Skill));
                RefreshUI();
            }
        }
        
        private int GetCurrentCost()
            => _unit.SkillStorage.skills.Sum(skill => skill.SkillCost);
    }
}