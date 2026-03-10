using System;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class SkillManageComponent : MonoBehaviour
    {
        private BasicUnitSkill usingSkill;

        public void SetSkillSO(SendSkillEvent skillSo)
        {
            usingSkill = skillSo.skill;
        }
    }
}