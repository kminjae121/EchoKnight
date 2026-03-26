using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.SkillSystem;
using UnityEngine;

public class AimArrow : BasicUnitSkill
{
    
    private UnitAnimation animtionCompo;

    private GameObject _target;

    private bool isHorizontal = false;

    private ShootItemAttackManager _shootItemManager;
    
    protected override void Start()
    {
        base.Start();
        triggerCompo.OnAimArrowTrigger += MakeArrow;
        triggerCompo.OnAimArrowEndTrigger += SkillEnd;
        SkillEvent.AddListener(AttackAction);
        animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        _shootItemManager = _owner.GetUnitCompo<ShootItemAttackManager>();
    }

    protected override void OnDestroy()
    {
        triggerCompo.OnAimArrowTrigger -= MakeArrow;
        triggerCompo.OnAimArrowEndTrigger -= SkillEnd;
        SkillEvent.RemoveListener(AttackAction);
        base.OnDestroy();
    }

    public void AttackAction(GameObject target)
    {
        StartCoroutine(FireArrowAction());
        SkillStartEvent?.Invoke();
        _target = target;
    }
    
    private IEnumerator FireArrowAction()
    {
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.1f);
        animtionCompo.PlaySelectAnimation("AIM");
    }

    protected override void SkillEnd()
    {
        base.SkillEnd();
        SkillEndEvent?.Invoke();
        animtionCompo.PlaySelectAnimation("IDLE");
    }
    
    public void MakeArrow()
    {
        impulseSource.GenerateImpulse(0.8f);  
        Vector3 pos = _unitBase.transform.position;

        pos.y += 2f;
            
        Vector3 slashRot = transform.rotation.eulerAngles;
        
        _shootItemManager.SetTarget(_target);
        _shootItemManager.SetDamageData(DamageData,AddDamage);
        _shootItemManager.CreateShootItem("AimArrow",pos, slashRot);
    
        _target = null;
    }
}