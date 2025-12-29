using System.Collections;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using Code.UnitSystem.SkillSystem.Skill;
using UnitSystem;
using UnityEngine;

public class SwordFlagSkill : BaseSkill
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnitAnimation animtionCompo;

    [SerializeField] private GameObject slashVFXPrefab;

    private GameObject _target;
    
 
    private void Start()
    {
        skillEvent.AddListener(UseSkill);
        _damageData.damage = 2.3456f;
        triggerCompo.OnSwordFlagSkillTrigger += MakeSlash;
        triggerCompo.OnSwordFlagSkillEndTrigger += TurnEnd;
    }

    private void UseSkill(GameObject target)
    {
        skillStartEvent?.Invoke();
        StartCoroutine(SlashFlag());
    }

    private IEnumerator SlashFlag()
    {
        yield return new WaitForSeconds(2f);
        animtionCompo.PlaySelectAnimation("SWORDFLAG");
    }

    public void MakeSlash()
    {
        Vector3 pos = transform.position;

        pos.y += 0.5f;
        
        GameObject slash = Instantiate(slashVFXPrefab, pos, Quaternion.identity);

        Vector3 slashRot = transform.rotation.eulerAngles;

        slashRot.y += 90;
        
        slash.transform.rotation = Quaternion.Euler(slashRot);
    }

    private void TurnEnd()
    {
        skillEndEvent?.Invoke();
    }
}