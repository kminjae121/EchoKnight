using System.Collections;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

    public class AimArrow : BasicUnitSkill
    {
        [SerializeField] private GameObject _ArrowPrefab;
        
        private UnitAnimation animtionCompo;

        private GameObject _target;
        
        private void Start()
        {
            triggerCompo.OnAimArrowTrigger += MakeArrow;
            triggerCompo.OnAimArrowEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnAimArrowTrigger -= MakeArrow;
            triggerCompo.OnAimArrowEndTrigger -= SkillEnd;
            skillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }

        public void AttackAction(GameObject target)
        {
            StartCoroutine(FireArrowAction());
            skillStartEvent?.Invoke();
            _target = target;
        }
        
        private IEnumerator FireArrowAction()
        {
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(0.1f);
            animtionCompo.PlaySelectAnimation("AIM");
        }

        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
        
        public void MakeArrow()
        {
            impulseSource.GenerateImpulse(0.8f);  
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            GameObject shootItem = Instantiate(_ArrowPrefab, pos, Quaternion.identity);
            ShootItem shootItemCompo = shootItem.GetComponent<ShootItem>();
            shootItemCompo.SetTarget(_target);
            shootItemCompo.SetDamageData(_damageData);
            Vector3 slashRot = transform.rotation.eulerAngles;
        
            shootItem.transform.rotation = Quaternion.Euler(slashRot);
            _target = null;
        }
    }