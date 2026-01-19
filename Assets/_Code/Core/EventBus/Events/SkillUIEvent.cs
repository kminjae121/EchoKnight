using Code.UnitSystem.SkillSystem;
using UnityEngine.UI;

namespace Code.Core.Events.Bus
{
    public struct SkillUIEvent : IEvent
    {
        public SkillComponent skillComponent;
        public int skillIdx;
        public string skillName;
        public Image skillImage;

        public SkillUIEvent(int skillIdx, string skillName, Image skillImage, SkillComponent skillComponent)
        {
            this.skillIdx = skillIdx;
            this.skillComponent = skillComponent;
            this.skillName = skillName;
            this.skillImage = skillImage;
        }
    }
}