using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class UnitSkillComponent : SkillComponent
    {
        protected override void StartSkill(BaseSkill skill, SkillSO skillso)
        {
            skill.ConfigureSkillRange(skillso);
            skill.ShowSkillRange();
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(false));
        }

        protected override void CancelSkill(BaseSkill skill)
        {
            skill.SkillFinished();
            skill.BooleanSkillUse(false);
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
        }
    }
}