using System.Collections;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

    public class FireBallSkill : BasicUnitSkill
    {
        [SerializeField] private GameObject fireBallPrefab;
        private UnitAnimation animtionCompo;

        private GameObject _target = null;

        protected override void Start()
        {
            base.Start();
            triggerCompo.OnFireBallTrigger += MakeArrow;
            triggerCompo.OnFireBallEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
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
        
        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
        
        public void MakeArrow()
        {
            impulseSource.GenerateImpulse(0.5f);  
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            fireBallPrefab.transform.position = pos;
            fireBallPrefab.SetActive(true);
            
            ShootItem shootItemCompo = fireBallPrefab.GetComponent<ShootItem>();
            shootItemCompo.SetTarget(_target);
            shootItemCompo.SetDamageData(_damageData);


            Vector3 slashRot = transform.rotation.eulerAngles;
        
            fireBallPrefab.transform.rotation = Quaternion.Euler(slashRot);
            
            _target = null;
        }
    }