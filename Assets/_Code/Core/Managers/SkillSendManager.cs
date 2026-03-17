using System.Collections.Generic;
using System.Linq;
using Code.Core.Managers;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace Code.Core.Managers
{
    public class SkillSendManager : MonoSingleton<SkillSendManager>
    {
        [Header("Skill Storage")]
        private readonly List<SkillSO> skills = new List<SkillSO>();
        private readonly Dictionary<UnitType, List<SkillSO>> _equipSkillDict = new Dictionary<UnitType, List<SkillSO>>();
        
        public void AddSkillList(SkillSO skill)
        {
            if (skill == null) return;
            if (skills.Contains(skill)) return;

            skills.Add(skill);
        }

        public List<SkillSO> GetSkillList(UnitType unitType)
        {
            if (skills == null) return new List<SkillSO>();
            return skills.Where(s => s != null && s.unitType == unitType).ToList();
        }
        
        public void EquipSkill(SkillSO skill)
        {
            if (skill == null) return;
            if (!skills.Contains(skill)) return;

            var unitType = skill.unitType;

            if (!_equipSkillDict.TryGetValue(unitType, out var list))
            {
                list = new List<SkillSO>();
                _equipSkillDict.Add(unitType, list);
            }
            
            if (list.Contains(skill)) return;

            list.Add(skill);
        }

        public void RemoveSkill(SkillSO skill)
        {
            if (skill == null) return;

            foreach (var skillList in _equipSkillDict.Values)
            {
                if (skillList.Contains(skill))
                {
                    skillList.Remove(skill);
                }
            }
        }

        public SkillSO[] GetEquipSkills(UnitType unitType)
        {
            if (!_equipSkillDict.TryGetValue(unitType, out var list))
            {
                return System.Array.Empty<SkillSO>();
            }

            return list.ToArray();
        }
    }
}