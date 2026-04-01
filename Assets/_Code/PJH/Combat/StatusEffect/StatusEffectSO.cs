using UnityEngine;

namespace Code.Combat.StatusEffect
{
    [CreateAssetMenu(fileName = "StatusEffect", menuName = "SO/StatusEffect", order = 0)]
    public class StatusEffectSO : ScriptableObject
    {
        public string effectName;
        public string description;
        public string className;
        public EffectPolarity polarity;
        public EffectType effectType;
        public Sprite effectIcon;
    }
}