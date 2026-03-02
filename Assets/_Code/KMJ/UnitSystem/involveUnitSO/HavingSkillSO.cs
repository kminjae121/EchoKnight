using System.Collections.Generic;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.involveUnitSO
{
    [CreateAssetMenu(fileName = "HavingSkillSO", menuName = "SkillSO", order = 0)]
    public class HavingSkillSO : ScriptableObject
    {
        public List<SkillSO> HaveSkills = new List<SkillSO>();
    }
}