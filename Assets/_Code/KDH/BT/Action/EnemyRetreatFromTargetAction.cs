using System;
using EnemySystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Retreat From Target", story: "[Agent] retreats from [Target]", category: "Action", id: "f577f05a03542ef3f525d069c7c53d56")]
public partial class EnemyRetreatFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> RetreatSteps = new BlackboardVariable<int>(3);

    private EnemyGridMovingSystem _mover;
    private bool _isMoving;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) 
        {
            return Status.Failure;
        }
        
        _mover = Agent.Value.GetComponent<EnemyGridMovingSystem>();

        if (_mover == null) 
        {
            Debug.LogError("[EnemyRetreatAction] Agent에 EnemyGridMovingSystem 컴포넌트가 없습니다.");
            return Status.Failure;
        }

        if (_mover.OnMoveEndEvent == null)
        {
            _mover.OnMoveEndEvent = new UnityEngine.Events.UnityEvent();
        }
        
        _mover.OnMoveEndEvent.AddListener(OnDone);
        
        _mover.RetreatFromTarget(Target.Value.transform.position, RetreatSteps.Value);
        
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
        if (_mover != null) 
            _mover.OnMoveEndEvent.RemoveListener(OnDone);
    }

    private void OnDone() 
    { 
        _isMoving = false; 
    }
}

