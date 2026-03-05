using System.Collections;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using Code.UnitSystem.SkillSystem.Skill;
using UnitSystem;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SwordFlagSkill : BaseSkill
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Animator animator;
    private UnitAnimation animtionCompo;

    [SerializeField] private GameObject slashVFXPrefab;

    public UnityEvent SwordFlagStartEvent;
    private GameObject _target;
    
 
    protected override void Start()
    {
        skillEvent.AddListener(UseSkill);
        _damageData.damage = 2.3456f;
        impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
        triggerCompo.OnSwordFlagSkillTrigger += MakeSlash;
        triggerCompo.OnSwordFlagSkillTrigger += CamShaking;
        triggerCompo.OnSwordFlagSkillEndTrigger += TurnEnd;
        animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        skillEvent.RemoveListener(UseSkill);
        triggerCompo.OnSwordFlagSkillTrigger -= MakeSlash;
        triggerCompo.OnSwordFlagSkillTrigger -= CamShaking;
        triggerCompo.OnSwordFlagSkillEndTrigger -= TurnEnd;
    }


    private void UseSkill(GameObject target)
    {
        skillStartEvent?.Invoke();
        StartCoroutine(SlashFlag());
    }
    private void CamShaking()
    {
        impulseSource.GenerateImpulse(0.6f);
    }

    private IEnumerator SlashFlag()
    {
        yield return new WaitForSeconds(2f);
        SwordFlagStartEvent?.Invoke();
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
}