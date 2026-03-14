using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.UnitSystem.SkillSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace _Code.Core.Managers
{
    public class SkillSendManager : MonoSingleton<SkillSendManager>
    {
        private readonly List<SkillSO> skills = new List<SkillSO>();
        
        private readonly Dictionary<UnitType, List<SkillSO>> equipSkillDict = new Dictionary<UnitType, List<SkillSO>>();
        
        public void AddSkillList(SkillSO skill)
        {
            if (skill == null) return;
            if (skills.Contains(skill)) return;

            skills.Add(skill);
        }
        
        public void EquipSkill(SkillSO skill)
        {
            if (skill == null) return;

            var unitType = skill.unitType;

            if (!equipSkillDict.TryGetValue(unitType, out var list))
            {
                list = new List<SkillSO>();
                equipSkillDict.Add(unitType, list);
            }
            
            if (list.Contains(skill)) return;

            list.Add(skill);
        }

        
        public void RemoveSkill(SkillSO skill)
        {
            foreach (var skills in equipSkillDict.Values)
            {
                if (skills.Contains(skill))
                    skills.Remove(skill);
            }
        }

        public SkillSO[] GetEquipSkills(UnitType unitType)
        {
            if (!equipSkillDict.TryGetValue(unitType, out var list))
                return System.Array.Empty<SkillSO>();

            return list.ToArray();
        }
    }
}