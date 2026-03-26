using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.SkillSystem;
using UnityEngine;
using UnityEngine.AI;

public class ShooterAttack : BasicUnitSkill
    {
        [SerializeField] private float atkMoveSpeed;
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        
         private UnitAnimation _animationCompo;

        private ShootItemAttackManager _shootItemManager;
        
        private GameObject _target = null;

        protected override void Start()
        {
            base.Start();
            SkillEvent.AddListener(AttackAction);
            _shootItemManager = _unitBase.GetUnitCompo<ShootItemAttackManager>();
            _animationCompo = _unitBase.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SkillEvent.RemoveListener(AttackAction);
        }

        protected override void StartEvent()
        {
            base.StartEvent();
            triggerCompo.OnAttackTrigger += Shoot;
            triggerCompo.OnAnimationEndTrigger += SkillEnd;
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
            _shootItemManager.SetDamageData(DamageData,AddDamage);
            _shootItemManager.CreateShootItem("ShootItem",pos, slashRot);   

            _characterUnit.impulseSource.GenerateImpulse(0.3f);
        }

        protected override void SkillEnd()
        {
            base.SkillEnd();
            triggerCompo.OnAttackTrigger -= Shoot;
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            _animationCompo.PlaySelectAnimation("IDLE");
            SkillEndEvent.Invoke();
        }
    }