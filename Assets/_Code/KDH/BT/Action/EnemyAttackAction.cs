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
        if (Agent.Value == null || Target.Value == null) 
        {
            return Status.Failure;
        }

        _enemyUnit = Agent.Value.GetComponent<EnemyUnit>();
        if (_enemyUnit == null)
        {
            Debug.LogError($"[EnemyAttackAction] {Agent.Value.name}에 EnemyUnit 컴포넌트가 없습니다.");
            return Status.Failure;
        }

        _isAttacking = true;
        
        // 스킬 이름이 비어있을 경우에 대한 방어 로직은 EnemyUnit.OrderSkill 내부에서 처리됨
        string skillToUse = SkillName.Value;
        
        _enemyUnit.OrderSkill(skillToUse, Target.Value, OnDone);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isAttacking) return Status.Running;
        return Status.Success;
    }

    private void OnDone() { _isAttacking = false; }
}