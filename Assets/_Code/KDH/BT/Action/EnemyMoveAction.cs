using System;
using EnemySystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Move To Target", story: "[Agent] moves [MinMoveStep] ~ [MaxMoveStep] steps towards [Target]", category: "Action", id: "fae195f5aaf05b504e77f14d366dc7bc")]
public partial class EnemyMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    
    [SerializeReference] public BlackboardVariable<int> MinMoveStep; 
    [SerializeReference] public BlackboardVariable<int> MaxMoveStep;

    private EnemyUnit _enemyUnit;
    private bool _isMoving;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        
        _enemyUnit = Agent.Value.GetComponent<EnemyUnit>();
        if (_enemyUnit == null) 
        {
            Debug.LogError($"[EnemyMoveAction] {Agent.Value.name}에 Enemy 컴포넌트 없음.");
            return Status.Failure;
        }

        int min = MinMoveStep.Value > 0 ? MinMoveStep.Value : 1;
        int max = MaxMoveStep.Value >= min ? MaxMoveStep.Value : min;
        int steps = UnityEngine.Random.Range(min, max + 1);

        _isMoving = true;

        _enemyUnit.OrderMove(Target.Value.transform.position, steps, OnDone);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isMoving) return Status.Running;
        return Status.Success;
    }

    private void OnDone() { _isMoving = false; }
}