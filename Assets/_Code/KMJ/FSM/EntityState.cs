using Code.AttackSystem;
using Code.UnitSystem;

namespace Blade.FSM
{
    public abstract class EntityState
    {
        protected Unit _entity;
        protected int _animationHash;
        protected UnitAnimationTrigger _animatorTrigger; 
        protected bool _isTriggerCall;

        public EntityState(Unit entity, int animationHash)
        {
            _entity = entity;
            _animationHash = animationHash;
            _animatorTrigger = entity.GetUnitCompo<UnitAnimationTrigger>(); 
        }

        public virtual void Enter()
        {
            _isTriggerCall = false;
            _animatorTrigger.OnAnimationEndTrigger += AnimationEndTrigger; 
        }

        public virtual void Update() { }

        public virtual void Exit()
        {
            _animatorTrigger.OnAnimationEndTrigger -= AnimationEndTrigger;
        }

        public virtual void AnimationEndTrigger() => _isTriggerCall = true;
    }
}