using System;
using Code.UnitSystem;
using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitAnimationTrigger : MonoBehaviour,IUnitComponent
    {
        public Action OnAnimationEndTrigger;
            
        public Action OnAttackTrigger;
        
        public Action OnTakeDamageTrigger;

        public Action OnBaseAttackSkillTrigger;

        public Action OnBaseAttackSkillEndTrigger;
        
        public Action OnSwordFlagSkillTrigger;
        
        private Unit _entity;
        
        public void Initialize(Unit  entity)
        {
            _entity = entity;
        }

        private void AnimationEnd()
        {
            OnAnimationEndTrigger?.Invoke();
        }
        
        private void MeleeAttack() => OnAttackTrigger?.Invoke();

        private void GetDamage() => OnTakeDamageTrigger?.Invoke();

        private void BasicAttackSkill() => OnBaseAttackSkillTrigger?.Invoke();
        private void SwordFlagSkill() => OnSwordFlagSkillTrigger?.Invoke();

        private void BasicAttackEndSkill() => OnBaseAttackSkillEndTrigger?.Invoke();

    }
}