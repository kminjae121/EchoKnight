using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.Events.Bus
{
    public struct SkillUIEvent : IEvent
    {
        public SkillComponent SkillComponent { get; private set; }
        public int SkillIndex { get; private set; }
        public string SkillName { get; private set; }
        public Sprite SkillImage { get; private set; }
        public float SkillCost { get; private set; }

        public SkillUIEvent(int skillIndex, string skillName, float skillCost,Sprite skillImage, SkillComponent skillComponent)
        {
            SkillIndex = skillIndex;
            SkillComponent = skillComponent;
            SkillName = skillName;
            SkillImage = skillImage;
            SkillCost = skillCost;
        }
    }
}