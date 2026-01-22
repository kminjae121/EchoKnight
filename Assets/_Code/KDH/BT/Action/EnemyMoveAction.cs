using System;
using EnemySystem;
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

    private EnemyGridMovingSystem _mover;
    private bool _isMoving;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        _mover = Agent.Value.GetComponent<EnemyGridMovingSystem>();
        
        _mover.MoveTowardsTarget(Target.Value.transform.position);
        _mover.OnMoveEndEvent.AddListener(OnDone);
        _isMoving = true;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isMoving) return Status.Running;
        return Status.Success;
    }

    protected override void OnEnd()
    {
        if (_mover != null) _mover.OnMoveEndEvent.RemoveListener(OnDone);
    }

    private void OnDone() { _isMoving = false; }
}

