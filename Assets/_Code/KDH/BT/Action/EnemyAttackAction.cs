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
        if (Agent.Value == null)
        {
            Debug.LogError("[EnemyAttackAction] Agent(행동 주체)가 할당되지 않았습니다. BT 변수 매핑을 확인하세요.");
            return Status.Failure;
        }
        
        if (Target.Value == null)
        {
            Debug.LogError("[EnemyAttackAction] Target(공격 대상)이 할당되지 않았습니다. FindTarget 노드가 실패했거나 변수가 연결되지 않았습니다.");
            return Status.Failure;
        }

        _enemyUnit = Agent.Value.GetComponent<EnemyUnit>();
        if (_enemyUnit == null)
        {
            Debug.LogError($"[EnemyAttackAction] {Agent.Value.name}에 EnemyUnit 컴포넌트가 없습니다.");
            return Status.Failure;
        }

        _isAttacking = true;
        
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