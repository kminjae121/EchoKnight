using System;
using System.Collections;
using System.Collections.Generic;
using Code.EntityComponent;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

public class BasicAttackSkill : BaseSkill
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnitAnimation animtionCompo;
    [SerializeField] private UnitAnimationTrigger triggerCompo;

    [SerializeField] private float atkMoveSpeed;
    
    [SerializeField] private float attackMoveDistance = 1.5f;
        
    private Vector3 _ownTrm;
    private void Start()
    {
        skillEvent.AddListener(AttackAction);
        triggerCompo.OnBaseAttackSkillEndTrigger += AttackEnd;
        triggerCompo.OnBaseAttackSkillTrigger += TakeDamage;
        _damageData.damage = 2.3456f;
    }
    
    
    public void AttackAction(GameObject target)
    {
        _ownTrm = transform.position;
        StartCoroutine(MeleeAttackAction(target));
    }

    private IEnumerator MeleeAttackAction(GameObject target)
    {
        yield return new WaitForSeconds(1.3f);
            
        animtionCompo.PlaySelectAnimation("MOVE");
            
        while (Vector3.Distance(gameObject.transform.position, target.transform.position) > attackMoveDistance)
        {
            Vector3 currentPos = gameObject.transform.position;
            Vector3 targetPos = target.transform.position;
                
            targetPos.y = currentPos.y;

            gameObject.transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                atkMoveSpeed * Time.deltaTime
            );

            yield return null;
        }
            
        animtionCompo.PlaySelectAnimation("BAS");
    }
    
    public void TakeDamage()
    {
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
            
        while (Vector3.Distance(gameObject.transform.position, _ownTrm) > 0.01f)
        {
            gameObject.transform.position = Vector3.MoveTowards(
                gameObject.transform.position,
                _ownTrm,
                atkMoveSpeed * Time.deltaTime
            );
            yield return null;
        }
        animtionCompo.PlaySelectAnimation("IDLE");
        
        skillEndEvent?.Invoke();
    }
}
