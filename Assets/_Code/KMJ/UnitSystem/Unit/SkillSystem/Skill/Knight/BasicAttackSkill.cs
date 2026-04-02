using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.SkillSystem;
using UnityEngine;

public class BasicAttackSkill : BasicUnitSkill
{
    [SerializeField] private Animator animator;
    private UnitAnimation animtionCompo;

    [SerializeField] private float atkMoveSpeed;
    
    [SerializeField] private float attackMoveDistance = 1.5f;

    private GameObject _target;
        
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
        _target = null;
        _target = target;
        StartCoroutine(MeleeAttackAction(_targetEnemy)); ;
    }

    private IEnumerator MeleeAttackAction(GameObject target)
    {
        yield return new WaitForSeconds(0.4f);
        
        animtionCompo.PlaySelectAnimation("BAS");
    }
    
    public void TakeDamage()
    {
        Bus<CamShakeEvent>.Raise(new CamShakeEvent(0.45f));
        Bus<DamageEvent>.Raise(new DamageEvent(DamageData,attackData,_target,AddDamage,_characterUnit,false));
    }

    public void AttackEnd()
    {
        ReturnOwnPos();
    }

    private void ReturnOwnPos()
    {
        animtionCompo.PlaySelectAnimation("IDLE");
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
