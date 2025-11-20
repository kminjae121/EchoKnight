using UnityEngine;

namespace Skill
{
    [CreateAssetMenu (fileName = "Unit", menuName = "Unit/UnitSkillCard")]
    public class UnitSkillCardSO : ScriptableObject
    {
        public string skillCardName;

        public float coolTime;
    }
}