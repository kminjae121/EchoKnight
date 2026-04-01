using Code.UnitSystem;

namespace Code.Combat.StatusEffect
{
    public abstract class StatusEffect
    {
        public StatusEffectSO StatusEffectSO { get; private set; }
        public EffectPolarity Polarity { get; private set; }
        public EffectType EffectType { get; private set; }
        
        public int? Duration { get; private set; }
        public int? Value { get; private set; }

        public bool IsCompleted => Duration <= 0;

        protected Unit _target;

        public void Initialize(StatusEffectSO statusEffectSO)
        {
            StatusEffectSO = statusEffectSO;
            EffectType = statusEffectSO.effectType;
            Polarity = statusEffectSO.polarity;
        }

        public virtual void SetEffect(Unit target, StatusEffectApplyData data)
        {
            _target = target;
            Duration = data.Duration;
            Value = data.Value;
        }

        public virtual void UpdateEffect()
        {
            if (_target == null || Duration <= 0)
                return;

            if (Duration != null)
                --Duration;
        }

        public virtual void EndEffect()
        {
            _target = null;
        }

        // 실제 버프 / 디버프 로직
        public abstract void ApplyEffect();

        // 중복 효과 처리
        public abstract void Merge(StatusEffectApplyData data);
    }
}