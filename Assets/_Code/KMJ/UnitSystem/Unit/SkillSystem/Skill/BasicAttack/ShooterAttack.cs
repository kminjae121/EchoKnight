using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

    public class ShooterAttack : BasicUnitSkill
    {
        [SerializeField] private float atkMoveSpeed;

        [SerializeField] private Animator animator;

         private UnitAnimation _animationCompo;

        private ShootItemAttackManager _shootItemManager;
        
        private GameObject _target = null;

        protected override void Start()
        {
            base.Start();
            triggerCompo.OnShootAttackTrigger += Shoot;
            triggerCompo.OnShootAttackEndTrigger += AttackEnd;
            skillEvent.AddListener(AttackAction);
            _shootItemManager = _unitBase.GetUnitCompo<ShootItemAttackManager>();
            _animationCompo = _unitBase.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            triggerCompo.OnShootAttackTrigger -= Shoot;
            triggerCompo.OnShootAttackEndTrigger -= AttackEnd;
            skillEvent.RemoveListener(AttackAction);
        }

        public void AttackAction(GameObject target)
        {
            StartCoroutine(ShootAttackSet(target));
        }

        private IEnumerator ShootAttackSet(GameObject target)
        {
            yield return new WaitForSeconds(0.4f);
            
            _target = null;
            _target = target;
            
            _animationCompo.PlaySelectAnimation("ATTACK");
        }

        private void Shoot()
        {
            Vector3 pos = _unitBase.transform.position;

            pos.y += 1.6f;
            
            Vector3 slashRot = _unitBase.transform.rotation.eulerAngles;
            
            _shootItemManager.SetTarget(_target);
            _shootItemManager.SetDamageData(DamageData);
            _shootItemManager.CreateShootItem("ShootItem",pos, slashRot);   

            _characterUnit.impulseSource.GenerateImpulse(0.3f);
        }
        
        private void AttackEnd()
        {
            _animationCompo.PlaySelectAnimation("IDLE");
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            
            skillEndEvent.Invoke();
        }
    }