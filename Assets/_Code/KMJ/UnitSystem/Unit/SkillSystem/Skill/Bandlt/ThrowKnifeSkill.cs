using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

    public class ThrowKnifeSkill : BaseSkill
    {
        private UnitAnimation animtionCompo;

        private GameObject _target;

        private ShootItemAttackManager _shootItemManager;
        
        protected override void Start()
        {
            base.Start();
            SkillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
            _shootItemManager = _owner.GetUnitCompo<ShootItemAttackManager>();
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
            SkillStartEvent?.Invoke();
        }
        
        private IEnumerator SlashFlag()
        {
            yield return new WaitForSeconds(0.4f);
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