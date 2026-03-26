using System.Collections;
using Code.Core.Events.Bus;
using Code.Managers;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using Code.UnitSystem.SkillSystem.Skill.Knight;
using UnityEngine;


public class ShieldSkill : BasicUnitSkill
{
    [SerializeField] private GameObject effectPrefab;

    private KnightShieldCompo _shieldCompo;
    private UnitAnimation animtionCompo;

    private int turnCnt = 0;

    protected override void Start()
    {
        base.Start();
        SkillEvent.AddListener(AddAP);
        animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        _shieldCompo = _characterUnit.GetComponentInChildren<KnightShieldCompo>();
        triggerCompo.OnSheldEndEvent += SkillEnd;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SkillEvent.RemoveListener(AddAP);
        triggerCompo.OnSheldEndEvent -= SkillEnd;

    }

    private void AddAP(GameObject obj)
    {
        SkillStartEvent?.Invoke();
        StartCoroutine(Shield());
    }

    public void SetShield()
    {
        TurnManager.Instance.OnTurnStart += HandleShieldEvent;
    }

    private void HandleShieldEvent()
    {
        if (turnCnt >= 1)
        {
            TurnManager.Instance.OnTurnStart -= HandleShieldEvent;
            _characterUnit.InitilizeDefensivePower();
            _shieldCompo.SetBaseMaterial();
            return;
        }
        
        _shieldCompo.SetChangeMaterial();
        turnCnt+=1;
        _characterUnit.AddDefensivePower += 10;
        _characterUnit.unitSO.DefensivePower += _characterUnit.AddDefensivePower;
    }

    private IEnumerator Shield()
    {
        yield return new WaitForSeconds(0.4f);
        effectPrefab.SetActive(true);
        SetShield();
        animtionCompo.PlaySelectAnimation("SHELD");
        effectPrefab.GetComponent<ParticleSystem>().Play();
    }
    
    protected override void SkillEnd()
    {
        base.SkillEnd();
        Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
        SkillEndEvent?.Invoke();
        animtionCompo.PlaySelectAnimation("IDLE");
    }
}