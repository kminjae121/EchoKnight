using System;
using EnemySystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Retreat From Target", story: "[Agent] retreats [MinStep] to [MaxStep] steps from [Target]", category: "Action", id: "EnemyRetreatFromTargetAction")]
public partial class EnemyRetreatFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> MinStep = new BlackboardVariable<int>(1);
    [SerializeReference] public BlackboardVariable<int> MaxStep = new BlackboardVariable<int>(3);

    private EnemyUnit _enemyUnit;
    private bool _isMoving;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        _enemyUnit = Agent.Value.GetComponent<EnemyUnit>();
        if (_enemyUnit == null)
        {
            return Status.Failure;
        }

        int randomSteps = Random.Range(MinStep.Value, MaxStep.Value + 1);

        _isMoving = true;
        
        _enemyUnit.OrderRetreat(Target.Value.transform.position, randomSteps, OnDone);

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