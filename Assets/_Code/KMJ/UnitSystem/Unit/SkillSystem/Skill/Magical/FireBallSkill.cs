using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

    public class FireBallSkill : BasicUnitSkill
    { 
        private UnitAnimation animtionCompo;

        private GameObject _target = null;

        private ShootItemAttackManager _shootItemManager;

        protected override void Start()
        {
            base.Start();
            SkillType = SkillType.ActiveSkill;
            triggerCompo.OnFireBallTrigger += MakeArrow;
            triggerCompo.OnFireBallEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
            
            _shootItemManager = _owner.GetUnitCompo<ShootItemAttackManager>();
        }

        protected override void OnDestroy()
        { 
            triggerCompo.OnFireBallTrigger -= MakeArrow;
            triggerCompo.OnFireBallEndTrigger -= SkillEnd;
            skillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
            
        }
        
        public void AttackAction(GameObject target)
        {
            StartCoroutine(FireBall());
            _target = target;
            skillStartEvent?.Invoke();
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
            skillEndEvent?.Invoke();
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