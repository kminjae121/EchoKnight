using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class SkillManageComponent : MonoBehaviour
    {
        private BasicUnitSkill usingSkill;

        private void Awake()
        {
            Bus<SendSkillEvent>.Subscribe(SetSkillSO);
        }

        private void OnDisable()
        {
            Bus<SendSkillEvent>.Unsubscribe(SetSkillSO);
        }

        public void SetSkillSO(SendSkillEvent skillSo)
        {
            usingSkill = skillSo.skill;
        }

        public BasicUnitSkill GetSkillInfo()
        {
            return usingSkill;
        }
    }
}