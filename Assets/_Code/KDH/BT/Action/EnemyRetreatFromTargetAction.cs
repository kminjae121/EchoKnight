using System;
using EnemySystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Retreat From Target", story: "[Agent] retreats from [Target]", category: "Action", id: "EnemyRetreatAction")]
public partial class EnemyRetreatFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> RetreatSteps = new BlackboardVariable<int>(3);

    private EnemyUnit _enemyUnit;
    private bool _isMoving;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) 
            return Status.Failure;
        
        _enemyUnit = Agent.Value.GetComponent<EnemyUnit>();

        if (_enemyUnit == null) 
        {
            Debug.LogError($"[EnemyRetreatAction] {Agent.Value.name}에 EnemyUnit 컴포넌트가 없습니다.");
            return Status.Failure;
        }

        _isMoving = true;
        
        _enemyUnit.OrderRetreat(Target.Value.transform.position, RetreatSteps.Value, OnDone);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isMoving) return Status.Running;
        return Status.Success;
    }

    private void OnDone()
    {
        _isMoving = false;
    }
}