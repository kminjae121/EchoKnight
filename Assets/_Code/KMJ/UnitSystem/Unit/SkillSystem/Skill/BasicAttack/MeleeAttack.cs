using System.Collections;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

    public class MeleeAttack : BasicUnitSkill
    { 
        private UnitAnimation _animationCompo;
        
        [SerializeField] private Animator animator;
        
        [SerializeField] private float atkMoveSpeed;
        [SerializeField] private float attackMoveDistance = 1.5f;

        [SerializeField] private AttackDataSO atkData;
        
        public bool isRunningAttack = false;
        
        private Vector3 _ownTrm;

        private GameObject _target = null;
        
        protected override void Start()
        {
            base.Start();
            triggerCompo.OnTakeDamageTrigger += TakeDamage;
            triggerCompo.OnAttackTrigger += AttackEnd;
            skillEvent.AddListener(AttackAction);
            _animationCompo = _unitBase.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnTakeDamageTrigger -= TakeDamage;
            triggerCompo.OnAttackTrigger -= AttackEnd;
            skillEvent.RemoveListener(AttackAction);
        }

        public void AttackAction(GameObject target)
        {
            _ownTrm = _unitBase.transform.position;
            _target = target;
            
            StartCoroutine(MeleeAttackAction(target));
        }

        private IEnumerator MeleeAttackAction(GameObject target)
        {
            yield return new WaitForSeconds(0.4f);
            
            _animationCompo.PlaySelectAnimation("MOVE");
            
            while (Vector3.Distance(target.transform.position, _unitBase.transform.position) > attackMoveDistance)
            {
                Vector3 currentPos = _unitBase.transform.position;
                Vector3 targetPos = target.transform.position;
                
                targetPos.y = currentPos.y;

                _unitBase.transform.position = Vector3.MoveTowards(
                    currentPos,
                    targetPos,
                    atkMoveSpeed * Time.deltaTime
                );
                if(isRunningAttack && Vector3.Distance(_unitBase.transform.position, target.transform.position) 
                   < attackMoveDistance * 2.67)
                    _animationCompo.PlaySelectAnimation("ATTACK");

                yield return null;
            }
            if(isRunningAttack == false)
             _animationCompo.PlaySelectAnimation("ATTACK");
        }

        public void AttackEnd()
        {
            StartCoroutine(ReturnOwnPos());
        }

        private IEnumerator ReturnOwnPos()
        {
            _animationCompo.PlaySelectAnimation("MOVE");

            while (Vector3.Distance(_unitBase.transform.position, _ownTrm) > 0.01f)
            {
                _unitBase.transform.position = Vector3.MoveTowards(
                    _unitBase.transform.position,
                    _ownTrm,
                    atkMoveSpeed * Time.deltaTime
                );
                yield return null;
            }

            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, 
                false,new Vector3(0.1f,0.1f,0.1f)));
            
            _animationCompo.PlaySelectAnimation("IDLE");
            skillEndEvent?.Invoke();
            
             Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
        
        public void TakeDamage()
        {
            Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
            _characterUnit.impulseSource.GenerateImpulse(0.6f);  
            
            
            _target.GetComponent<EntityHealth>().ApplyDamage(DamageData, 
                _target.transform.position,transform.position,atkData,_characterUnit);
            
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false)); 
        }
    }