using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct SkillUIHoverEvent : IEvent
    {
        public SkillSO Skill { get; }
        public RectTransform Transform { get; }

        public SkillUIHoverEvent(SkillSO skill, RectTransform transform)
        {
            Skill = skill;
            Transform = transform;
        }
    }
}