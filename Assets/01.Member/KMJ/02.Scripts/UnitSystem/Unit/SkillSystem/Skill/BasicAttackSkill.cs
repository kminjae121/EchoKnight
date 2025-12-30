using System;
using System.Collections;
using System.Collections.Generic;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.EntityComponent;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

public class BasicAttackSkill : BaseSkill
{
    
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Animator animator;
    [SerializeField] private UnitAnimation animtionCompo;

    [SerializeField] private float atkMoveSpeed;
    
    [SerializeField] private float attackMoveDistance = 1.5f;
        
    private Vector3 _ownTrm;
    private void Start()
    {
        skillEvent.AddListener(AttackAction);
        triggerCompo.OnBaseAttackSkillEndTrigger += AttackEnd;
        triggerCompo.OnBaseAttackSkillTrigger += TakeDamage;
        impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
        Debug.Log(impulseSource.gameObject.name);
        _damageData.damage = 2.3456f;
    }
    
    
    public void AttackAction(GameObject target)
    {
        _ownTrm = _owner.transform.position;
        StartCoroutine(MeleeAttackAction(target));
        skillStartEvent?.Invoke();
    }

    private IEnumerator MeleeAttackAction(GameObject target)
    {
        yield return new WaitForSeconds(2.2f);
            
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

            if (Vector3.Distance(_owner.transform.position, target.transform.position) < attackMoveDistance * 2)
            {
                animtionCompo.PlaySelectAnimation("BAS");
            }

            yield return null;
        }
    }
    
    public void TakeDamage()
    {
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
        
        //skillEndEvent?.Invoke();
    }
}
