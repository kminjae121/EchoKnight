using System;

namespace Code.Combat.StatusEffect
{
    [Flags]
    public enum EffectType
    {
        None = 0,
        Poison = 1 << 1,
        Stunned = 1 << 2
    }
}