using System.Collections;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

public class SwordFlagSkill : BaseSkill
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnitAnimation animtionCompo;
 
    private void Start()
    {
        skillEvent.AddListener(UseSkill);
        _damageData.damage = 2.3456f;
        triggerCompo.OnSwordFlagSkillTrigger += TurnEnd;
    }

    private void UseSkill(GameObject target)
    {
        animtionCompo.PlaySelectAnimation("SWORDFLAG");
    }

    private void TurnEnd()
    {
        skillEndEvent?.Invoke();
    }
}