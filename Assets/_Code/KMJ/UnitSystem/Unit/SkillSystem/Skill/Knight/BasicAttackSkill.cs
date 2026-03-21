using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.GimicSystem;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

public class BasicAttackSkill : BasicUnitSkill
{
    [SerializeField] private Animator animator;
    private UnitAnimation animtionCompo;

    [SerializeField] private float atkMoveSpeed;
    
    [SerializeField] private float attackMoveDistance = 1.5f;
        
    private Vector3 _ownTrm;
    
    protected override void Start()
    {
        base.Start();
        SkillType = SkillType.ActiveSkill;
        skillEvent.AddListener(AttackAction);
        triggerCompo.OnBaseAttackSkillEndTrigger += AttackEnd;
        triggerCompo.OnBaseAttackSkillTrigger += TakeDamage;
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
        
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.1f);
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
        _characterUnit.impulseSource.GenerateImpulse(0.3f);

        ;
        Bus<DamageEvent>.Raise(new DamageEvent(DamageData,attackData,_targetEnemy,AddDamage,_characterUnit,false));
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
        Bus<UseGimicEvent>.Raise(new UseGimicEvent(UnitType.Knight, null));
        _targetEnemy = null;
        SkillEnd();
    }

    protected override void SkillEnd()
    {
        base.SkillEnd();
        skillEndEvent.Invoke();
    }
}
