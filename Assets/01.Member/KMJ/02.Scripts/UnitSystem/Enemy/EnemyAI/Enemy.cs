using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;

public class Enemy : Unit
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Dead()
    {
        TestDeath();
        base.Dead();
    }

    public override void OnTurnStart()
    {
        Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
        
        base.OnTurnStart();
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
    }

    public void TestDeath()
    {
        gameObject.SetActive(false);
    }
}
