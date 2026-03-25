using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.AI;

public class LongAttack : BasicUnitSkill
    {
        [SerializeField] private float atkMoveSpeed;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private NavMeshAgent agent;
        
        private UnitAnimation _animationCompo;

        
        private GameObject _target = null;
        
        public bool isRunningAttack = false;
        
        private Vector3 _ownTrm;

        protected override void Start()
        {
            base.Start();
            triggerCompo.OnLongRangeAttackTrigger += ShootLongRangeAttack;
            triggerCompo.OnLongRangeAttackEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            _animationCompo = _unitBase.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            triggerCompo.OnLongRangeAttackTrigger -= ShootLongRangeAttack;
            triggerCompo.OnLongRangeAttackEndTrigger -= SkillEnd;
            skillEvent.RemoveListener(AttackAction);
        }

        public void AttackAction(GameObject target)
        {
            agent.enabled = false;
            _ownTrm = transform.position;
            
            StartCoroutine(MeleeAttackAction(target));
        }

        private IEnumerator MeleeAttackAction(GameObject target)
        {
            yield return new WaitForSeconds(0.4f);
            
             _target = target;
             
             _animationCompo.PlaySelectAnimation("ATTACK");
        }


        private void ShootLongRangeAttack()
        {
            Vector3 dir = _target.transform.position;
            dir.y += 1.4f;
            
            effectPrefab.GetComponent<BoomingEffect>().SetDamageData(DamageData,AddDamage);
            effectPrefab.transform.position = dir;
            effectPrefab.SetActive(true);
        }


        protected override void SkillEnd()
        {
            base.SkillEnd();
            _animationCompo.PlaySelectAnimation("IDLE");
            _characterUnit.BehaviorCompo.IsActive = true;
            skillEndEvent?.Invoke();
        }
    }