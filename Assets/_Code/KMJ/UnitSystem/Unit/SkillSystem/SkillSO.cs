using UnityEngine;
using UnityEngine.UI;

namespace Code.UnitSystem.SkillSystem
{
    [CreateAssetMenu(fileName = "SkillSO/Skill", menuName = "skillSO", order = 0)]
    public class SkillSO : ScriptableObject
    {
        public UnitType unitType = UnitType.None;
        public string skillName;
        public int SkillCost;
        public Image skillUIImage;
        public string className;
    }
}