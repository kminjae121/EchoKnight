using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    [CreateAssetMenu(fileName = "SkillSO/Skill", menuName = "skillSO", order = 0)]
    public class SkillSO : ScriptableObject
    {
        [Header("Basic Info")]
        public UnitType unitType = UnitType.None;
        public string skillName;
        [TextArea(3, 15)]
        public string SkillDescription;
        public int SkillCost;
        public int UsingSkillCost;
        public Sprite skillUIImage;
        public string className;
        public int skillPrice;

        [Header("Detail Info")]
        public float SkillDamage;
        public float SkillRange;
    }
}