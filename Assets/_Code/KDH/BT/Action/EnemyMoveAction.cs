using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Move To Target", story: "[Agent] moves towards [Target]", category: "Action", id: "fae195f5aaf05b504e77f14d366dc7bc")]
public partial class EnemyMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private Enemy _enemy;
    private bool _isMoving;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        
        _enemy = Agent.Value.GetComponent<Enemy>();
        if (_enemy == null) 
        {
            Debug.LogError($"[EnemyMoveAction] {Agent.Value.name}에 Enemy 컴포넌트 없음.");
            return Status.Failure;
        }

        _isMoving = true;
        
        // Enemy의 통합 메서드 호출
        _enemy.MoveToTarget(Target.Value.transform.position, OnDone);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isMoving) return Status.Running;
        return Status.Success;
    }

    private void OnDone() { _isMoving = false; }
}

