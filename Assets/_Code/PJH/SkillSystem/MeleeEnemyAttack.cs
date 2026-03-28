using Code.Core.Debugs;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.SkillSystem
{
    public class MeleeEnemyAttack : BaseSkill
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
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            base.OnDestroy();
        }

        private void AttackAction(GameObject target)
        {
            _target = target;
        }
        
        private void TakeDamage()
        {
            // 임펄스랑 데미지
            UnityLogger.Log("적이 데미지를 주었습니다.");
            Bus<DamageEvent>.Raise(new DamageEvent(DamageData, attackData, _target, AddDamage,null , false));
        }
        
        private void SkillEnd()
        {
            triggerCompo.OnAttackTrigger -= TakeDamage;
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
        }
    }
}