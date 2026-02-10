using System;
using System.Linq;
using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class CharacterSkillInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI SkillCostText;
        [SerializeField] private CharacterSkillButton SkillPrefab;

        private UnitSO _unit;

        private void Awake()
        {
            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
        }
        
        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit.Data;

            RefreshUI();
        }
        
        private void RefreshUI()
        {
            int skillCost = 0;
            
            foreach (var skill in _unit.OwnSkillStorage.skills)
            {
                var skillObj = Instantiate(SkillPrefab);
                
                if (_unit.SkillStorage.skills.Contains(skill))
                {
                    //skillCost = skill.
                    skillObj.SetSkill(skill, true);
                }
                else
                {
                    skillObj.SetSkill(skill, false);
                }
                
                
            }
            
            SkillCostText.text = $"0 / {_unit.Cost}";
        }
    }
}