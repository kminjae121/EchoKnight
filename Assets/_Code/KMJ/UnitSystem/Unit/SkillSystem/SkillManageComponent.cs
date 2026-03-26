using System;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class SkillManageComponent : MonoBehaviour
    {
        private BaseSkill usingSkill;

        private void OnEnable()
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

        public BaseSkill GetSkillInfo()
        {
            return usingSkill;
        }
    }
}