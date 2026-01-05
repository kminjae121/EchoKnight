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
            AnimationAllStop();
            
            _animator.SetBool(animationName,true);
        }

        public void ReturnIdleAnimation()
        {
            AnimationAllStop();

            _animator.SetBool("IDLE", true);
        }

        public void AnimationAllStop()
        {
            if (_animator == null) return;

            var parameters = _animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                
                if (p.type == AnimatorControllerParameterType.Bool)
                {
                    _animator.SetBool(p.name, false);
                }
            }
        }
    }
}