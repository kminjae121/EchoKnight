using Code.UnitSystem.SkillSystem;

namespace Code.Core.Events.Bus
{
    public struct SendSkillEvent : IEvent
    {
        public BaseSkill skill;

        public SendSkillEvent(BaseSkill skill)
        {
            this.skill = skill;
        }
    }
}