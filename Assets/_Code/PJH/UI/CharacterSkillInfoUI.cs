using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class CharacterSkillInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI SkillCostText;
        [SerializeField] private CharacterSkillButton SkillPrefab;
        [SerializeField] private Transform skillTrm;

        private UnitSO _unit;

        private void Awake()
        {
            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            Bus<SkillEquipEvent>.Subscribe(SkillEquip);
            Bus<SkillUnequipEvent>.Subscribe(SkillUnequip);
            
            gameObject.SetActive(false);
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
        
        public void ActivePanel()
        {
            gameObject.SetActive(true);
        }
        
        private void RefreshUI()
        {
            foreach (Transform child in skillTrm)
                Destroy(child.gameObject);

            int skillCost = 0;

            foreach (var skill in _unit.OwnSkillStorage.skills)
            {
                bool equipped = _unit.SkillStorage.skills.Contains(skill);

                if (equipped)
                    skillCost += skill.SkillCost;

                var skillButton = Instantiate(SkillPrefab, skillTrm);
                skillButton.SetSkill(skill, equipped);
            }

            SkillCostText.text = $"{skillCost} / {_unit.Cost}";
        }
        
        private void SkillEquip(SkillEquipEvent evt)
        {
            if (_unit.SkillStorage.skills.Contains(evt.Skill))
                return;

            _unit.SkillStorage.skills.Add(evt.Skill);
            RefreshUI();
        }

        private void SkillUnequip(SkillUnequipEvent evt)
        {
            if (_unit.SkillStorage.skills.Remove(evt.Skill))
                RefreshUI();
        }
    }
}