using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.UnitSystem;

namespace Blade.FSM
{
    public abstract class EntityState
    {
        protected Unit _entity;
        protected int _animationHash;
        protected UnitAnimator _entityAnimator;
        protected UnitAnimationTrigger _animatorTrigger; 
        protected bool _isTriggerCall;

        public EntityState(Unit entity, int animationHash)
        {
            _entity = entity;
            _animationHash = animationHash;
            _entityAnimator = entity.GetUnitCompo<UnitAnimator>();
            _animatorTrigger = entity.GetUnitCompo<UnitAnimationTrigger>(); 
        }

        public virtual void Enter()
        {
            _entityAnimator.SetParam(_animationHash, true);
            _isTriggerCall = false;
            _animatorTrigger.OnAnimationEndTrigger += AnimationEndTrigger; 
        }

        public virtual void Update() { }

        public virtual void Exit()
        {
            _entityAnimator.SetParam(_animationHash, false);
            _animatorTrigger.OnAnimationEndTrigger -= AnimationEndTrigger;
        }

        public virtual void AnimationEndTrigger() => _isTriggerCall = true;
    }
}