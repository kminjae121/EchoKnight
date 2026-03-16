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
            _shootItemManager.SetDamageData(DamageData);
            _shootItemManager.CreateShootItem("Knife",pos, slashRot);

            _target = null;
        }
        
        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
    }