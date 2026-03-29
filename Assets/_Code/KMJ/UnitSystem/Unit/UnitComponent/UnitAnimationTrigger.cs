using System;
using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitAnimationTrigger : MonoBehaviour, IUnitComponent
    {
        public Action OnDeadEvent;
        public Action OnAttackTrigger;
        public Action OnAnimationEndTrigger;
        public Action OnEnemyAnimationEndTrigger;
        public Action OnTakeDamageTrigger;
        public Action OnEnemyDieEndTrigger;
        
        
        private Unit _entity;
        
        public void Initialize(Unit entity)
        {
            _entity = entity;
        }

        private void TakeDamage() => OnTakeDamageTrigger?.Invoke();

        public void TriggerDead() => OnDeadEvent?.Invoke();
        
        public void TriggerEnemyIdle() => OnEnemyAnimationEndTrigger?.Invoke();
        
        public void TriggerEnemyDead() => OnEnemyDieEndTrigger?.Invoke();
        
        private void AnimationEnd() => OnAnimationEndTrigger?.Invoke();
        
        private void Dead() => OnDeadEvent?.Invoke();
        private void EnemyDead() => OnEnemyDieEndTrigger?.Invoke();
        private void Attack() => OnAttackTrigger?.Invoke();
        private void EnemyAnimationEnd() => OnEnemyAnimationEndTrigger?.Invoke();
    }
}