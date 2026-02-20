using System.Collections;
using System.Collections.Generic;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

    public class ThrowKnifeSkill : BasicUnitSkill
    {
        [SerializeField] private GameObject _knifePrefab;
        
        private UnitAnimation animtionCompo;

        private GameObject _target;
        
        protected override void Start()
        {
            base.Start();
            triggerCompo.OnThrowKnifeTrigger += MakeThrowKnife;
            triggerCompo.OnThrowKnifeEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
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
        
            GameObject shootItem = Instantiate(_knifePrefab, pos, Quaternion.identity);

            ShootItem shootItemCompo = shootItem.GetComponent<ShootItem>();
            shootItemCompo.SetTarget(_target);
            shootItemCompo.SetDamageData(_damageData);
            
            Vector3 slashRot = transform.rotation.eulerAngles;
        
            shootItem.transform.rotation = Quaternion.Euler(slashRot);
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