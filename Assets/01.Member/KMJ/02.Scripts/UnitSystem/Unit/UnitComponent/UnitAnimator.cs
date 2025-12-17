using Code.UnitSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem
{
    public class UnitAnimator : MonoBehaviour, IUnitComponent
    {
        public UnityEvent<Vector3, Quaternion> OnAnimatorMoveEvent;
        [SerializeField] private Animator animator;

        public bool ApplyRootMotion
        {
            get => animator.applyRootMotion;
            set => animator.applyRootMotion = value;
        }
        
        private Unit _entity;

        public void Initialize(Unit entity)
        {
            _entity = entity;
        }

        private void OnAnimatorMove()
        {
            OnAnimatorMoveEvent?.Invoke(animator.deltaPosition, animator.deltaRotation);
        }

        public void SetParam(int hash, float value) => animator.SetFloat(hash, value);
        public void SetParam(int hash, bool value) => animator.SetBool(hash, value);
        public void SetParam(int hash, int value) => animator.SetInteger(hash, value);
        public void SetParam(int hash) => animator.SetTrigger(hash);

        public void SetParam(int hash, float value, float dampTime)
            => animator.SetFloat(hash, value, dampTime, Time.deltaTime);

        public void SetAnimatorOff()
        {
            animator.enabled = false;
        }
    }
}