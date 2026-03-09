using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.UnitSystem;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent
{
    public class MeleeAttacker : MonoBehaviour
    {
        [SerializeField] private UnitAttackComponent atkCompo;
        [SerializeField] private UnitAnimation animtionCompo;
        [SerializeField] private UnitAnimationTrigger triggerCompo;
        
        [SerializeField] private Animator animator;
        
        [SerializeField] private float atkMoveSpeed;
        [SerializeField] private float attackMoveDistance = 1.5f;

        [SerializeField] private AttackDataSO atkData;
        
        public bool isRunningAttack = false;
        
        private Vector3 _ownTrm;

        private GameObject _target = null;

        private void Start()
        {
            triggerCompo.OnTakeDamageTrigger += TakeDamage;
            triggerCompo.OnAttackTrigger += AttackEnd;
            atkCompo.attackEvent.AddListener(AttackAction);
        }

        private void OnDestroy()
        {
            triggerCompo.OnTakeDamageTrigger -= TakeDamage;
            triggerCompo.OnAttackTrigger -= AttackEnd;
            atkCompo.attackEvent.RemoveListener(AttackAction);
        }

        public void AttackAction(GameObject target)
        {
            _ownTrm = transform.position;
            _target = target;
            
            StartCoroutine(MeleeAttackAction(target));
        }

        private IEnumerator MeleeAttackAction(GameObject target)
        {
            yield return new WaitForSeconds(0.4f);
            
            animtionCompo.PlaySelectAnimation("MOVE");
            
            while (Vector3.Distance(target.transform.position, gameObject.transform.position) > attackMoveDistance)
            {
                Vector3 currentPos = gameObject.transform.position;
                Vector3 targetPos = target.transform.position;
                
                targetPos.y = currentPos.y;

                gameObject.transform.position = Vector3.MoveTowards(
                    currentPos,
                    targetPos,
                    atkMoveSpeed * Time.deltaTime
                );
                if(isRunningAttack && Vector3.Distance(gameObject.transform.position, target.transform.position) 
                   < attackMoveDistance * 2.67)
                    animtionCompo.PlaySelectAnimation("ATTACK");

                yield return null;
            }
            if(isRunningAttack == false)
             animtionCompo.PlaySelectAnimation("ATTACK");
        }

        public void AttackEnd()
        {
            StartCoroutine(ReturnOwnPos());
        }

        private IEnumerator ReturnOwnPos()
        {
            animtionCompo.PlaySelectAnimation("MOVE");

            while (Vector3.Distance(gameObject.transform.position, _ownTrm) > 0.01f)
            {
                gameObject.transform.position = Vector3.MoveTowards(
                    gameObject.transform.position,
                    _ownTrm,
                    atkMoveSpeed * Time.deltaTime
                );
                yield return null;
            }

            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, 
                false,new Vector3(0.1f,0.1f,0.1f)));
            
            animtionCompo.PlaySelectAnimation("IDLE");
            atkCompo.attackEndEvent?.Invoke();
            
             Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
        
        public void TakeDamage()
        {
            Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
            atkCompo._characterUnit.impulseSource.GenerateImpulse(0.6f);  
            
            _target.GetComponent<EntityHealth>().ApplyDamage(atkCompo._damageData, 
                _target.transform.position,transform.position,atkData,atkCompo._characterUnit);
            
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false)); 
        }
    }
}