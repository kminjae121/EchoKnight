using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnityEngine;


public class SheldSkill : BasicUnitSkill
{
    [SerializeField] private GameObject effectPrefab;
    
    private UnitAnimation animtionCompo;

    protected override void Start()
    {
        base.Start();
        SkillType = SkillType.ActiveSkill;
        skillEvent.AddListener(AddAP);
        animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        
        triggerCompo.OnSheldEvent += PlusSheld;

        triggerCompo.OnSheldEndEvent += SkillEnd;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        skillEvent.RemoveListener(AddAP);
        triggerCompo.OnSheldEndEvent -= SkillEnd;
        triggerCompo.OnSheldEvent -= PlusSheld;
    }

    private void AddAP(GameObject obj)
    {
        skillStartEvent?.Invoke();
        StartCoroutine(Sheld());
    }

    private IEnumerator Sheld()
    {
        yield return new WaitForSeconds(0.4f);
        effectPrefab.SetActive(true);
        animtionCompo.PlaySelectAnimation("SHELD");
        effectPrefab.GetComponent<ParticleSystem>().Play();
    }

    private void PlusSheld()
    {
        CharacterUnit unit = _owner as CharacterUnit;
        unit.AddDefensivePower += 10;
        unit.unitSO.DefensivePower += unit.AddDefensivePower;
    }
    
    protected override void SkillEnd()
    {
        base.SkillEnd();
        Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
        skillEndEvent?.Invoke();
        animtionCompo.PlaySelectAnimation("IDLE");
    }
}