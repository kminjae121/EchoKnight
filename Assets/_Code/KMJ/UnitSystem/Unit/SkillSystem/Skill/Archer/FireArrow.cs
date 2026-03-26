using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

    public class FireArrow : BasicUnitSkill
    {
        private UnitAnimation animtionCompo;

        private GameObject _target;
        
        private ShootItemAttackManager  _shootItemManager;
        
        protected override void Start()
        {
            base.Start();
            triggerCompo.OnFireArrowTrigger += MakeArrow;
            triggerCompo.OnFireArrowEndTrigger += SkillEnd;
            SkillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
            
            _shootItemManager = _owner.GetUnitCompo<ShootItemAttackManager>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnFireArrowTrigger -= MakeArrow;
            triggerCompo.OnFireArrowEndTrigger -= SkillEnd;
            SkillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }

        public void AttackAction(GameObject target)
        {
            _target = null;
            StartCoroutine(FireArrowAction());
            _target = target;
            SkillStartEvent?.Invoke();
            
        }
        
        protected override void SkillEnd()
        {
            base.SkillEnd();
            SkillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
        }
        
        private IEnumerator FireArrowAction()
        {
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(0.1f);
            animtionCompo.PlaySelectAnimation("FIRE");
        }
        
        public void MakeArrow()
        {
            Vector3 pos = _unitBase.transform.position;

            pos.y += 2f;

            Vector3 slashRot = transform.rotation.eulerAngles;
            
            _shootItemManager.SetTarget(_target);
            _shootItemManager.SetDamageData(DamageData,AddDamage);
            _shootItemManager.CreateShootItem("FireArrow",pos, slashRot);
        }
    }