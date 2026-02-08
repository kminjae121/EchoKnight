using System;
using EnemySystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Skill Attack", story: "[Agent] uses skill [SkillName] on [Target]", category: "Action", id: "EnemyAttackAction")]
public partial class EnemyAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<string> SkillName;

    private EnemyUnit _enemyUnit;
    private bool _isAttacking;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;

        _enemyUnit = Agent.Value.GetComponent<EnemyUnit>();
        if (_enemyUnit == null)
        {
            Debug.LogError($"[EnemyAttackAction] {Agent.Value.name}에 Enemy 컴포넌트 없음.");
            return Status.Failure;
        }

        _isAttacking = true;
        
        _enemyUnit.OrderSkill(SkillName.Value, Target.Value, OnDone);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isAttacking) return Status.Running;
        return Status.Success;
    }

    private void OnDone() { _isAttacking = false; }
}