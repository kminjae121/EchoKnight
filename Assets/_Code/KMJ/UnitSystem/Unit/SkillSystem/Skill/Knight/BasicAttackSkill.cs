using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.GimicSystem;
using Code.SkillSystem;
using UnityEngine;

public class BasicAttackSkill : BasicUnitSkill
{
    [SerializeField] private Animator animator;
    private UnitAnimation animtionCompo;

    [SerializeField] private float atkMoveSpeed;
    
    [SerializeField] private float attackMoveDistance = 1.5f;
        
    private Vector3 _ownTrm;
    
    protected void Start()
    {
        SkillEvent.AddListener(AttackAction);
        animtionCompo = _characterUnit.GetUnitCompo<UnitAnimation>();
    }

    protected override void StartEvent()
    {
        base.StartEvent();
        triggerCompo.OnAnimationEndTrigger += AttackEnd;
        triggerCompo.OnAttackTrigger += TakeDamage;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        SkillEvent.RemoveListener(AttackAction);
    }
    


    public void AttackAction(GameObject target)
    {
        _ownTrm = _characterUnit.transform.position;
        StartCoroutine(MeleeAttackAction(target)); ;
        SkillStartEvent?.Invoke();
    }

    private IEnumerator MeleeAttackAction(GameObject target)
    {
        
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.1f);
        _targetEnemy = target;
            
        animtionCompo.PlaySelectAnimation("MOVE");
            
        while (Vector3.Distance(_characterUnit.transform.position, target.transform.position) > attackMoveDistance)
        {
            Vector3 currentPos = _characterUnit.transform.position;
            Vector3 targetPos = target.transform.position;
                
            targetPos.y = currentPos.y;

            _characterUnit.transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                atkMoveSpeed * Time.deltaTime
            );

            yield return null;
        }
        
        if (Vector3.Distance(_characterUnit.transform.position, target.transform.position) <= attackMoveDistance * 2)
        {
            animtionCompo.PlaySelectAnimation("BAS");
        }
    }
    
    public void TakeDamage()
    {
        Bus<CamShakeEvent>.Raise(new CamShakeEvent(0.3f));
        
        Bus<DamageEvent>.Raise(new DamageEvent(DamageData,attackData,_targetEnemy,AddDamage,_characterUnit,false));
    }

    public void AttackEnd()
    {
        StartCoroutine(ReturnOwnPos());
    }

    private IEnumerator ReturnOwnPos()
    {
        animtionCompo.PlaySelectAnimation("MOVE");
            
        while (Vector3.Distance(_characterUnit.transform.position, _ownTrm) > 0.01f)
        {
            _characterUnit.transform.position = Vector3.MoveTowards(
                _characterUnit.transform.position,
                _ownTrm,
                atkMoveSpeed * Time.deltaTime
            );
            yield return null;
        }
        animtionCompo.PlaySelectAnimation("IDLE");
        Bus<UseGimicEvent>.Raise(new UseGimicEvent(UnitType.Knight, null));
        _targetEnemy = null;
        SkillEnd();
    }

    protected override void SkillEnd()
    {
        base.SkillEnd();
        triggerCompo.OnAnimationEndTrigger -= AttackEnd;
        triggerCompo.OnAttackTrigger -= TakeDamage;
        SkillEndEvent.Invoke();
    }
}
