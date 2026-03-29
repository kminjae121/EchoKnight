using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.SkillSystem;
using UnityEngine;

    public class FireBallSkill : BasicUnitSkill
    { 
        private UnitAnimation animtionCompo;

        private GameObject _target = null;

        private ShootItemAttackManager _shootItemManager;

        protected void Start()
        {
            SkillEvent.AddListener(AttackAction);
            animtionCompo = _characterUnit.GetUnitCompo<UnitAnimation>();
            
            _shootItemManager = _characterUnit.GetUnitCompo<ShootItemAttackManager>();
        }

        protected override void StartEvent()
        {
            triggerCompo.OnAttackTrigger += MakeArrow;
            triggerCompo.OnAnimationEndTrigger += SkillEnd;
            base.StartEvent();
        }

        protected override void OnDestroy()
        { 
            SkillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }
        
        public void AttackAction(GameObject target)
        {
            StartCoroutine(FireBall());
            _target = target;
            SkillStartEvent?.Invoke();
        }
        
        private IEnumerator FireBall()
        {
           
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(0.1f);
            animtionCompo.PlaySelectAnimation("FIREBALL");
        }
        
        protected override void SkillEnd()
        {
            base.SkillEnd();
            triggerCompo.OnAttackTrigger -= MakeArrow;
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            SkillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
        }
        
        public void MakeArrow()
        {
            impulseSource.GenerateImpulse(0.5f);  
            Vector3 pos = transform.position;

            pos.y += 3f;

            Vector3 slashRot = transform.rotation.eulerAngles;
            
            _shootItemManager.SetTarget(_target);
            _shootItemManager.SetDamageData(DamageData,AddDamage);
            _shootItemManager.CreateShootItem("FireBall",pos, slashRot);
            
            _target = null;
        }
    }