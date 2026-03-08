using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace Code.Core.Events.Bus
{
    public class SkillEquipPopupEvent : IEvent
    {
        public SkillSO Skill { get; }
        public bool IsEquipped { get; }
        public Vector2 Position { get; }

        public SkillEquipPopupEvent(SkillSO skill, bool isEquipped, Vector2 position)
        {
            Skill = skill;
            IsEquipped = isEquipped;
            Position = position;
        }
    }
}