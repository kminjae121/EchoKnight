using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.Events.Bus
{
    public struct SkillUIEvent : IEvent
    {
        public int SkillIndex { get; private set; }
        public SkillSO SkillSO { get; private set; }
        public SkillComponent SkillComponent { get; private set; }

        public SkillUIEvent(int skillIndex, SkillSO skillInfo, SkillComponent skillComponent)
        {
            SkillIndex = skillIndex;
            SkillComponent = skillComponent;
            SkillSO = skillInfo;
        }
    }
}