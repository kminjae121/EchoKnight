using Code.UnitSystem.SkillSystem;

namespace Code.Core.Events.Bus
{
    public struct SkillUIHoverEvent : IEvent
    {
        public SkillSO Skill { get; }

        public SkillUIHoverEvent(SkillSO skill)
        {
            Skill = skill;
        }
    }
}