using System;
using Code.UnitSystem;
using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitAnimationTrigger : MonoBehaviour,IUnitComponent
    {
        #region Bandlt

        public Action OnThrowKnifeTrigger;

        public Action OnAddAPTrigger;

        #endregion

        #region Archer

        public Action OnFireArrowTrigger;
        
        public Action OnAimArrowTrigger;

        #endregion

        #region Magical

        public Action OnFireBallTrigger;
        public Action OnHealTrigger;

        #endregion
        public Action OnAnimationEndTrigger;

        public Action OnEnemyAnimationEndTrigger;

        public Action OnEnemyDieEndTrigger;
            
        public Action OnAttackTrigger;
        
        public Action OnTakeDamageTrigger;

        public Action OnBaseAttackSkillTrigger;

        public Action OnBaseAttackSkillEndTrigger;
        
        public Action OnSwordFlagSkillTrigger;

        public Action OnSwordFlagSkillEndTrigger;

        public Action OnLongRangeAttackTrigger;

        public Action OnShootAttackTrigger;

        public Action OnShootAttackEndTrigger;
        
        public Action OnLongRangeAttackEndTrigger;
        
        private Unit _entity;
        
        public void Initialize(Unit  entity)
        {
            _entity = entity;
        }

        private void AnimationEnd()
        {
            OnAnimationEndTrigger?.Invoke();
        }

        #region Bandlt

        private void ThrowKnife() => OnThrowKnifeTrigger?.Invoke();
        private void AddAP()=> OnAddAPTrigger?.Invoke();

        #endregion

        #region Archer

        private void FireArrow() => OnFireArrowTrigger?.Invoke();
        
        private void AimArrow() => OnAimArrowTrigger?.Invoke();

        #endregion
        
        #region Magical
        
        private void FireBall() => OnFireBallTrigger?.Invoke();

        private void Heal() => OnHealTrigger?.Invoke();
        #endregion

        private void LongRangeAttackEnd() => OnLongRangeAttackEndTrigger?.Invoke();

        private void ShootAttackEnd() => OnShootAttackEndTrigger?.Invoke();
        
        private void LongRangeAttack() => OnLongRangeAttackTrigger?.Invoke();

        private void ShootAttack() => OnShootAttackTrigger?.Invoke();

        private void EnemyDead() => OnEnemyDieEndTrigger?.Invoke();
        
        private void MeleeAttack() => OnAttackTrigger?.Invoke();

        private void GetDamage() => OnTakeDamageTrigger?.Invoke();

        private void BasicAttackSkill() => OnBaseAttackSkillTrigger?.Invoke();
        private void SwordFlagSkill() => OnSwordFlagSkillTrigger?.Invoke();
        
        private void SwordFlagSkillEnd() => OnSwordFlagSkillEndTrigger?.Invoke();

        private void BasicAttackEndSkill() => OnBaseAttackSkillEndTrigger?.Invoke();
        
        private void EnemyAnimationEnd() => OnEnemyAnimationEndTrigger?.Invoke();

    }
}