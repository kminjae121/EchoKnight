using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

    public class ThrowKnifeSkill : BasicUnitSkill
    {
        private UnitAnimation animtionCompo;

        private GameObject _target;

        private ShootItemAttackManager _shootItemManager;
        
        protected override void Start()
        {
            base.Start();
            SkillType = SkillType.ActiveSkill;
            triggerCompo.OnThrowKnifeTrigger += MakeThrowKnife;
            triggerCompo.OnThrowKnifeEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
            _shootItemManager = _owner.GetUnitCompo<ShootItemAttackManager>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnThrowKnifeTrigger -= MakeThrowKnife;
            triggerCompo.OnThrowKnifeEndTrigger -= SkillEnd;
            skillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }

        public void AttackAction(GameObject target)
        {
            StartCoroutine(SlashFlag());
            _target = target;
            skillStartEvent?.Invoke();
        }
        
        private IEnumerator SlashFlag()
        {
           
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(0.1f);
            animtionCompo.PlaySelectAnimation("THROW");
        }
        
        public void MakeThrowKnife()
        {
            impulseSource.GenerateImpulse(0.5f);  
            Vector3 pos = _unitBase.transform.position;

            pos.y += 2f;
        
            Vector3 slashRot = transform.rotation.eulerAngles;
            
            
            _shootItemManager.SetTarget(_target);
            _shootItemManager.SetDamageData(DamageData,AddDamage);
            _shootItemManager.CreateShootItem("Knife",pos, slashRot);

            _target = null;
        }
        
        protected override void SkillEnd()
        {
            base.SkillEnd();
            skillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
        }
    }