using UnityEngine;
using UnityEngine.UI;

namespace Code.UnitSystem.SkillSystem
{
    [CreateAssetMenu(fileName = "SkillSO/Skill", menuName = "skillSO", order = 0)]
    public class SkillSO : ScriptableObject
    {
        public UnitType unitType = UnitType.None;
        public string skillName;
        public string SkillDescription;
        public int SkillCost;
        public int UsingSkillCost;
        public Sprite skillUIImage;
        public string className;
    }
}