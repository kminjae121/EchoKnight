using Code.UnitSystem;
using UnityEngine;

namespace UnitSystem
{
    public class UnitAnimation : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private Animator _animator;
        
        public void Initialize(Unit owner)
        {
            AnimationAllStop();
        }

        public void PlaySelectAnimation(string animationName)
        {
            if (_animator == null) return;
            
            AnimationAllStop();
            _animator.SetBool(animationName, true);
        }

        public void ReturnIdleAnimation()
        {
            PlaySelectAnimation("IDLE");
        }

        public void AnimationAllStop()
        {
            if (_animator == null) return;

            foreach (var param in _animator.parameters)
                if (param.type == AnimatorControllerParameterType.Bool)
                    _animator.SetBool(param.name, false);
        }
        
        public void RestartFromEntry()
        {
            if (_animator == null) return;
            
            AnimationAllStop();
            ResetAllTriggers();
            
            _animator.Rebind();
            _animator.Update(0f);
        }
        
        private void ResetAllTriggers()
        {
            if (_animator == null) return;

            foreach (var param in _animator.parameters)
                if (param.type == AnimatorControllerParameterType.Trigger)
                    _animator.ResetTrigger(param.name);
        }
    }
}