using System;
using System.Collections;
using System.Collections.Generic;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

public class BasicAttackSkill : BaseSkill
{
    [SerializeField] private Animator animator;
    private UnitAnimation animtionCompo;

    [SerializeField] private float atkMoveSpeed;
    
    [SerializeField] private float attackMoveDistance = 1.5f;
        
    private Vector3 _ownTrm;
    
    private void Start()
    {
        skillEvent.AddListener(AttackAction);
        triggerCompo.OnBaseAttackSkillEndTrigger += AttackEnd;
        triggerCompo.OnBaseAttackSkillTrigger += TakeDamage;
        impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
        _damageData.damage = 7;
        animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        skillEvent.RemoveListener(AttackAction);
        triggerCompo.OnBaseAttackSkillEndTrigger -= AttackEnd;
        triggerCompo.OnBaseAttackSkillTrigger -= TakeDamage;
    }
    


    public void AttackAction(GameObject target)
    {
        _ownTrm = _owner.transform.position;
        StartCoroutine(MeleeAttackAction(target)); ;
        skillStartEvent?.Invoke();
    }

    private IEnumerator MeleeAttackAction(GameObject target)
    {
        yield return new WaitForSeconds(2.2f);
        _targetEnemy = target;
            
        animtionCompo.PlaySelectAnimation("MOVE");
            
        while (Vector3.Distance(_owner.transform.position, target.transform.position) > attackMoveDistance)
        {
            Vector3 currentPos = _owner.transform.position;
            Vector3 targetPos = target.transform.position;
                
            targetPos.y = currentPos.y;

            _owner.transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                atkMoveSpeed * Time.deltaTime
            );

            yield return null;
        }
        
        if (Vector3.Distance(_owner.transform.position, target.transform.position) <= attackMoveDistance * 2)
        {
            animtionCompo.PlaySelectAnimation("BAS");
        }
    }
    
    public void TakeDamage()
    {
        Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
        impulseSource.GenerateImpulse(0.6f);
        
        _targetEnemy.GetComponent<EntityHealth>().ApplyDamage(_damageData, 
            _targetEnemy.transform.position,transform.position,attackData,_owner);
    }

    public void AttackEnd()
    {
        StartCoroutine(ReturnOwnPos());
    }

    private IEnumerator ReturnOwnPos()
    {
        animtionCompo.PlaySelectAnimation("MOVE");
            
        while (Vector3.Distance(_owner.transform.position, _ownTrm) > 0.01f)
        {
            _owner.transform.position = Vector3.MoveTowards(
                _owner.transform.position,
                _ownTrm,
                atkMoveSpeed * Time.deltaTime
            );
            yield return null;
        }
        animtionCompo.PlaySelectAnimation("IDLE");
        Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
        skillEndEvent.Invoke();
        _targetEnemy = null;
    }
}
