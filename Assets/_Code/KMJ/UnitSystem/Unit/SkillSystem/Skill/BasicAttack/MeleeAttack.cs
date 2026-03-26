using System.Collections;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttack : BaseSkill
{ 
    [SerializeField] private Animator animator;
    [SerializeField] private float atkMoveSpeed;
    [SerializeField] private float attackMoveDistance = 1.5f;
    [SerializeField] private AttackDataSO atkData;
    [SerializeField] private NavMeshAgent agent;
    
    private UnitAnimation _animationCompo;
    
    public bool isRunningAttack = false;
    
    private Vector3 _ownTrm;
    
    private GameObject _target = null;
    
    protected override void Start()
    {
        base.Start();
        SkillEvent.AddListener(AttackAction);
        _animationCompo = _unitBase.GetUnitCompo<UnitAnimation>();
    }

    protected override void StartEvent()
    {
        base.StartEvent();
        triggerCompo.OnAttackTrigger += TakeDamage;
        triggerCompo.OnAnimationEndTrigger += AttackEnd;
    }

    protected override void OnDestroy()
    {
        SkillEvent.RemoveListener(AttackAction);
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
            if (isRunningAttack && Vector3.Distance(_unitBase.transform.position, target.transform.position) 
                < attackMoveDistance * 2f)
            {
                break;
            }
    
            yield return null;
        }
        
        _animationCompo.PlaySelectAnimation("ATTACK");
        
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
        
        _animationCompo.PlaySelectAnimation("IDLE");
        SkillEnd();
    }
    
    protected override void SkillEnd()
    {
        base.SkillEnd();
        triggerCompo.OnAttackTrigger -= TakeDamage;
        triggerCompo.OnAnimationEndTrigger -= AttackEnd;
        SkillEndEvent?.Invoke();
    }
    
    public void TakeDamage()
    {
        _characterUnit.impulseSource.GenerateImpulse(0.6f);  
        
        Bus<DamageEvent>.Raise(new DamageEvent(DamageData,atkData,_target,AddDamage, _characterUnit, _characterUnit.IsConfirmationSkill));
    }
}