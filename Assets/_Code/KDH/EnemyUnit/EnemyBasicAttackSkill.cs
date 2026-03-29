using Code.UnitSystem;
using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.SkillSystem
{
    public class EnemyBasicAttackSkill : BaseSkill, IAfterInitialize
    {
        [SerializeField] private Unit _owner;
        
        public void AfterInitialize()
        {
            Damage = 10f; 
            SkillSO.UsingSkillCost = 0; 

            if (_owner != null)
            {
                if (triggerCompo == null) 
                    triggerCompo = _owner.GetUnitCompo<UnitAnimationTrigger>();
                
                if (RotationCompo == null) 
                    RotationCompo = _owner.GetUnitCompo<UnitRotation>();
                
                if (_skillCompo == null) 
                    _skillCompo = _owner.GetUnitCompo<SkillComponent>();
            }

            if (triggerCompo != null)
            {
               // triggerCompo.OnBaseAttackSkillTrigger -= AttackAction;
               // triggerCompo.OnBaseAttackSkillEndTrigger -= AttackEnd;
               //
               // triggerCompo.OnBaseAttackSkillTrigger += AttackAction; 
               // triggerCompo.OnBaseAttackSkillEndTrigger += AttackEnd;
            }
        }

        protected void Start()
        {
            AfterInitialize();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (triggerCompo != null)
            {
              //  triggerCompo.OnBaseAttackSkillTrigger -= AttackAction;
              //  triggerCompo.OnBaseAttackSkillEndTrigger -= AttackEnd;
            }
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
                Debug.LogError($"[EnemyBasicAttackSkill] 애니메이션 컴포넌트를 찾을 수 없어 턴을 강제 종료합니다.");
                SkillFinished();
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
                    damageData.damage = Damage;
                    damageData.isCritical = false;

                    Vector3 hitPoint = _targetEnemy.transform.position;
                    Vector3 hitNormal = (_targetEnemy.transform.position - _owner.transform.position).normalized;

                    targetHealth.ApplyDamage(damageData, hitPoint, hitNormal, attackData, _owner);
                }
            }
        }

        private void AttackEnd()
        {
            SkillFinished();
        }
    }
}