using System;
using _Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.UnitSystem;
using EnemySystem;
using UnitSystem;
using Unity.Behavior;
using UnityEngine;

public class Enemy : Unit
{
    [SerializeField] private BehaviorGraphAgent behaviorAgent;
    [SerializeField] private UnitAnimation animationCompo;
    [SerializeField] private UnitAnimationTrigger triggerCompo;
    [SerializeField] private EnemyGridMovingSystem moveCompo;
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Awake()
    {
        if (behaviorAgent == null)
            behaviorAgent = GetComponent<BehaviorGraphAgent>();
        
        if (behaviorAgent != null)
            behaviorAgent.BlackboardReference.SetVariableValue("IsMyTurn", false);
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
        gameObject.SetActive(false);
        StageManager.Instance.RemoveEnemy(this.gameObject);
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

        for (int i = 0; i <= 2; i++)
        {
            Bus<SkillUIEvent>.Raise(new SkillUIEvent(i, null, null, null));
        }
        
        if (behaviorAgent != null)
        {
            behaviorAgent.BlackboardReference.SetVariableValue("IsMyTurn", true);
        }
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
        animationCompo.PlaySelectAnimation("IDLE");
        animationCompo.PlaySelectAnimation("DIE");
    }
}
