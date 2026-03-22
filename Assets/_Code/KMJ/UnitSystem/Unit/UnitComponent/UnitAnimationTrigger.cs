using System;
using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitAnimationTrigger : MonoBehaviour, IUnitComponent
    {
        #region Bandlt
        public Action OnThrowKnifeTrigger;
        public Action OnThrowKnifeEndTrigger;
        public Action OnAddAPTrigger;
        public Action OnAddAPEndTrigger;
        #endregion

        #region Archer
        public Action OnFireArrowTrigger;
        public Action OnFireArrowEndTrigger;
        public Action OnAimArrowTrigger;
        public Action OnAimArrowEndTrigger;
        #endregion

        #region Magical
        public Action OnFireBallTrigger;
        public Action OnFireBallEndTrigger;
        public Action OnHealTrigger;
        public Action OnHealEndTrigger;
        #endregion

        #region Knight

        public Action OnSheldEvent;
        public Action OnSheldEndEvent;

        #endregion

        public Action OnDeadEvent;
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
        
        public void Initialize(Unit entity)
        {
            _entity = entity;
        }
        
        public void TriggerAttack() => OnBaseAttackSkillTrigger?.Invoke();
        
        public void TriggerAttackEnd() => OnBaseAttackSkillEndTrigger?.Invoke();

        public void TriggerDead() => OnDeadEvent?.Invoke();
        
        public void TriggerEnemyIdle() => OnEnemyAnimationEndTrigger?.Invoke();
        
        public void TriggerEnemyDead() => OnEnemyDieEndTrigger?.Invoke();
        
        private void AnimationEnd() => OnAnimationEndTrigger?.Invoke();

        #region Bandlt
        private void ThrowKnife() => OnThrowKnifeTrigger?.Invoke();
        private void ThrowKnifeEnd() => OnThrowKnifeEndTrigger?.Invoke();
        private void AddAP()=> OnAddAPTrigger?.Invoke();
        private void AddAPEnd()=> OnAddAPEndTrigger?.Invoke();
        #endregion

        #region Archer
        private void FireArrow() => OnFireArrowTrigger?.Invoke();
        private void FireArrowEnd() => OnFireArrowEndTrigger?.Invoke();
        private void AimArrow() => OnAimArrowTrigger?.Invoke();
        private void AimArrowEnd() => OnAimArrowEndTrigger?.Invoke();
        #endregion
        
        #region Magical
        private void FireBall() => OnFireBallTrigger?.Invoke();
        private void FireBallEnd() => OnFireBallEndTrigger?.Invoke();
        private void Heal() => OnHealTrigger?.Invoke();
        private void HealEnd() => OnHealEndTrigger?.Invoke();
        #endregion

        #region Knight
        private void Sheld() => OnSheldEvent?.Invoke();
        private void SheldEnd() => OnSheldEndEvent?.Invoke();

        #endregion
        
        private void Dead() => OnDeadEvent?.Invoke();
        private void LongRangeAttackEnd() => OnLongRangeAttackEndTrigger?.Invoke();
        private void ShootAttackEnd() => OnShootAttackEndTrigger?.Invoke();
        private void LongRangeAttack() => OnLongRangeAttackTrigger?.Invoke();
        private void ShootAttack() => OnShootAttackTrigger?.Invoke();
        private void EnemyDead() => OnEnemyDieEndTrigger?.Invoke();
        private void MeleeAttack() => OnAttackTrigger?.Invoke();
        private void GetDamage() => OnTakeDamageTrigger?.Invoke();
        private void SwordFlagSkill() => OnSwordFlagSkillTrigger?.Invoke();
        private void SwordFlagSkillEnd() => OnSwordFlagSkillEndTrigger?.Invoke();
        private void EnemyAnimationEnd() => OnEnemyAnimationEndTrigger?.Invoke();
        
        // 기존 이름 호환성 (혹시 모르니 유지)
        private void BasicAttackSkill() => OnBaseAttackSkillTrigger?.Invoke();
        private void BasicAttackEndSkill() => OnBaseAttackSkillEndTrigger?.Invoke();
    }
}