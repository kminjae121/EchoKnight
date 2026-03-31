using Code.UnitSystem;

namespace Code.Combat.StatusEffect
{
    public abstract class StatusEffect
    {
        public EffectPolarity Polarity { get; private set; }
        public EffectType EffectType { get; private set; }
        public int Duration { get; private set; }
        
        public bool IsActive => _target != null && Duration > 0;

        protected Unit _target;

        public void Initialize(EffectType effectType)
        {
            EffectType = effectType;
            Polarity = GetPolarity();
        }

        public virtual void ApplyEffect(Unit target, int duration)
        {
            _target = target;
            Duration = duration;
            OnApply();
        }

        public virtual void UpdateEffect()
        {
            if (_target == null || Duration <= 0)
                return;

            OnUpdate();
            --Duration;
        }

        public virtual void EndEffect()
        {
            if (_target == null)
                return;

            OnEnd();
            _target = null;
            Duration = 0;
        }

        public bool IsCompleted()
            => Duration <= 0;

        protected virtual EffectPolarity GetPolarity()
            => EffectPolarity.None;

        protected virtual void OnApply()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnEnd()
        {
        }
    }
}
