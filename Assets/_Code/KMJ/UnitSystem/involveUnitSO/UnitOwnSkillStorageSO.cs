using System.Collections.Generic;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.involveUnitSO
{
    [CreateAssetMenu(fileName = "UnitSO", menuName = "UnitSO/SkillOwnStorage")]
    public class UnitOwnSkillStorageSO : ScriptableObject
    {
        public UnitType uniType = UnitType.None;
        public List<SkillSO> skills = null;
        
        public HavingSkillSO havingSkill = null;
        
        private void OnValidate()
        {
            if (havingSkill == null || skills == null) return;
            
            var set = new HashSet<SkillSO>(havingSkill.HaveSkills);

            foreach (var skill in skills)
            {
                if (skill == null) continue;
                
                if (set.Add(skill))
                    havingSkill.HaveSkills.Add(skill);
            }
        }
    }
}