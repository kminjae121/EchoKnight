using System;
using Code.UnitSystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Attack", story: "[Agent] attacks [Target]", category: "Action", id: "715f65d5801182f191c85afc2bb6bff2")]
public partial class EnemyAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private UnitAttackComponent _attacker;
    private UnitRotation _rotator;
    private bool _isAttacking;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        _attacker = Agent.Value.GetComponent<UnitAttackComponent>();
        _rotator = Agent.Value.GetComponent<UnitRotation>();

        _attacker.attackEndEvent.AddListener(OnDone);
        
        // 타겟 방향 회전 후 공격
        if (_rotator != null) _rotator.SetDir(Target.Value.transform.position);
        _attacker.attackEvent?.Invoke(Target.Value);

        _isAttacking = true;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isAttacking) return Status.Running;
        return Status.Success;
    }

    protected override void OnEnd()
    {
        if (_attacker != null) _attacker.attackEndEvent.RemoveListener(OnDone);
    }

    private void OnDone() { _isAttacking = false; }
}

