using Code.EntityComponent;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace EnemySystem
{
    public class EnemyBasicAttackSkill : BaseSkill
    {
        protected override void Awake()
        {
            base.Awake();
            
            if (_owner != null)
            {
                if (triggerCompo == null) 
                    triggerCompo = _owner.GetUnitCompo<UnitAnimationTrigger>();
                
                if (rotationCompo == null) 
                    rotationCompo = _owner.GetUnitCompo<UnitRotation>();
                
                if (_skillCompo == null) 
                    _skillCompo = _owner.GetUnitCompo<SkillComponent>();
            }
        }

        private void OnEnable()
        {
            if (triggerCompo != null)
            {
                triggerCompo.OnBaseAttackSkillTrigger += AttackAction; 
                triggerCompo.OnBaseAttackSkillEndTrigger += AttackEnd;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            if (triggerCompo != null)
            {
                triggerCompo.OnBaseAttackSkillTrigger -= AttackAction;
                triggerCompo.OnBaseAttackSkillEndTrigger -= AttackEnd;
            }
        }

        public override void InitializeSkill()
        {
            base.InitializeSkill();
            
            damage = 10f; 
            useSkillPoint = 0; 
        }

        public override void ForceUseSkill(GameObject target)
        {
            base.ForceUseSkill(target);

            if (_owner != null && _owner.AnimationCompo != null)
            {
                _owner.AnimationCompo.PlaySelectAnimation("ATTACK");
            }
            else
            {
                Debug.LogError($"[EnemyBasicAttackSkill] {_owner.name}의 AnimationCompo가 없어 공격 애니메이션을 재생할 수 없습니다.");
                // 애니메이션이 없으면 즉시 종료 처리하여 멈춤 방지
                skillEnd();
            }
        }

        private void AttackAction()
        {
            AttackEnemy();
        }

        public override void AttackEnemy()
        {
            if (_targetEnemy != null)
            {
                var targetHealth = _targetEnemy.GetComponent<IDamageable>();
                if (targetHealth != null)
                {
                    DamageData damageData = new DamageData();
                    damageData.damage = damage;
                    damageData.isCritical = false;

                    Vector3 hitPoint = _targetEnemy.transform.position;
                    Vector3 hitNormal = (_targetEnemy.transform.position - _owner.transform.position).normalized;

                    targetHealth.ApplyDamage(damageData, hitPoint, hitNormal, attackData, _owner);
                }
            }
        }

        private void AttackEnd()
        {
            skillEnd();
        }
    }
}