using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.SkillSystem;
using UnityEngine;

    public class ThrowKnifeSkill : BasicUnitSkill
    {
        private UnitAnimation animtionCompo;

        private GameObject _target;

        private ShootItemAttackManager _shootItemManager;
        
        protected void Start()
        {
            SkillEvent.AddListener(AttackAction);
            animtionCompo = _characterUnit.GetUnitCompo<UnitAnimation>();
            _shootItemManager = _characterUnit.GetUnitCompo<ShootItemAttackManager>();
        }

        protected override void StartEvent()
        {
            base.StartEvent();
            triggerCompo.OnAttackTrigger += MakeThrowKnife;
            triggerCompo.OnAnimationEndTrigger += SkillEnd;
        }

        protected override void OnDestroy()
        {
            SkillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }

        public void AttackAction(GameObject target)
        {
            StartCoroutine(SlashFlag());
            _target = target;
        }
        
        private IEnumerator SlashFlag()
        {
            yield return new WaitForSeconds(0.4f);
            SkillFeedbackEvent?.Invoke();   
            animtionCompo.PlaySelectAnimation("THROW");
        }
        
        public void MakeThrowKnife()
        {
            _characterUnit.IsConfirmationSkill = true;    
        }
        
        protected override void SkillEnd()
        {
            base.SkillEnd();
            
            triggerCompo.OnAttackTrigger -= MakeThrowKnife;
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            SkillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
        }
    }