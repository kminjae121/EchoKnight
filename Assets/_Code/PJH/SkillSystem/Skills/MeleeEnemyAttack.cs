using Code.Core.Debugs;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.SkillSystem
{
    public class MeleeEnemyAttack : EnemyBaseSkill
    {
        private GameObject _target;

        protected void Start()
        {
            SkillEvent.AddListener(AttackAction);
        }

        protected override void StartEvent()
        {
            base.StartEvent();
            triggerCompo.OnAttackTrigger += TakeDamage;
            triggerCompo.OnAnimationEndTrigger += SkillEnd;
        }

        protected override void OnDestroy()
        {
            SkillEvent.RemoveListener(AttackAction);

            if (triggerCompo != null)
            {
                triggerCompo.OnAttackTrigger -= TakeDamage;
                triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            }

            base.OnDestroy();
        }

        private void AttackAction(GameObject target)
        {
            _target = target;
        }

        private void TakeDamage()
        {
            UnityLogger.Log("일반 공격으로 데미지");
            Bus<DamageEvent>.Raise(new DamageEvent(DamageData, attackData, _target, AddDamage, null, false,false,0.1f));
        }

        private void SkillEnd()
        {
            triggerCompo.OnAttackTrigger -= TakeDamage;
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            _target = null;
            SkillFinished(false);
            SkillEndEvent?.Invoke();
        }
    }
}
