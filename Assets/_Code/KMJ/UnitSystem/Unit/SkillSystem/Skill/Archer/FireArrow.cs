using System.Collections;
using System.Collections.Generic;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

    public class FireArrow : BasicUnitSkill
    {
        [SerializeField] private GameObject _ArrowPrefab;
        
        private UnitAnimation animtionCompo;

        private GameObject _target;
        
        protected override void Start()
        {
            base.Start();
            triggerCompo.OnFireArrowTrigger += MakeArrow;
            triggerCompo.OnFireArrowEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnFireArrowTrigger -= MakeArrow;
            triggerCompo.OnFireArrowEndTrigger -= SkillEnd;
            skillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }

        public void AttackAction(GameObject target)
        {
            _target = null;
            StartCoroutine(FireArrowAction());
            _target = target;
            skillStartEvent?.Invoke();
            
        }
        
        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
        
        private IEnumerator FireArrowAction()
        {
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(0.1f);
            animtionCompo.PlaySelectAnimation("FIRE");
        }
        
        public void MakeArrow()
        {
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            GameObject shootItem = Instantiate(_ArrowPrefab, pos, Quaternion.identity);

            Vector3 slashRot = transform.rotation.eulerAngles;
        
            ShootItem shootItemCompo = shootItem.GetComponent<ShootItem>();
            shootItemCompo.SetTarget(_target);
            shootItemCompo.SetDamageData(_damageData);
            shootItem.transform.rotation = Quaternion.Euler(slashRot);
        }
    }