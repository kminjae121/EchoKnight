using System;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.UnitSystem;
using EnemySystem;
using UnitSystem;
using UnityEngine;

public class Enemy : Unit
{

    [SerializeField] private UnitAnimation animationCompo;

    [SerializeField] private UnitAnimationTrigger triggerCompo;

    [SerializeField] private EnemyGridMovingSystem moveCompo;
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Start()
    {
        Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        animationCompo.PlaySelectAnimation("IDLE");
        triggerCompo.OnEnemyAnimationEndTrigger += ChangeIdle;
        triggerCompo.OnEnemyDieEndTrigger += Die;
    }

    private void Die()
    {
        Bus<EnemyDieEvent>.Raise(new EnemyDieEvent(1));
        gameObject.SetActive(false);
    }

    private void ChangeIdle()
    {
        animationCompo.PlaySelectAnimation("IDLE");
    }

    protected override void Dead()
    {
        DeadEnemy();
        base.Dead();
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        moveCompo.Move();
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
    }

    public void TurnEnd()
    {
        Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
    }

    public void EnemyHit()
    {
        animationCompo.PlaySelectAnimation("IDLE");
        animationCompo.PlaySelectAnimation("HIT");
    }

    public void DeadEnemy()
    {
        animationCompo.PlaySelectAnimation("DIE");
    }
}
