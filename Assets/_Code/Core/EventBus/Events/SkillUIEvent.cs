using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.Events.Bus
{
    public struct SkillUIEvent : IEvent
    {
        public SkillComponent skillComponent;
        public int skillIdx;
        public string skillName;
        public Sprite skillImage;

        public SkillUIEvent(int skillIdx, string skillName, Sprite skillImage, SkillComponent skillComponent)
        {
            this.skillIdx = skillIdx;
            this.skillComponent = skillComponent;
            this.skillName = skillName;
            this.skillImage = skillImage;
        }
    }
}