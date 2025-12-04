using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using EnemySystem;
using EntityComponent;
using UnitSystem;
using UnityEngine;

public class Enemy : Unit, ITurnable
{
    public bool IsPlayerUnit => isPlayerUnit;
        
    public float TurnGauge => turnGauge;

    public bool IsReadyDoAct => TurnGauge >= 100f;

    public float TurnSpeed => turnSpeed;

   
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Dead()
    {
        TestDealth();
        base.Dead();
    }

    public void TestDealth()
    {
        Bus<UnitDeadEvent>.Raise(new UnitDeadEvent(this));
        gameObject.SetActive(false);
    }
}
