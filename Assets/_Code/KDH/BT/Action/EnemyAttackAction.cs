using System;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
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

    private SkillComponent _skillComponent;
    private BaseSkill _currentSkill;
    private bool _isAttacking;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) 
            return Status.Failure;

        _skillComponent = Agent.Value.GetComponent<SkillComponent>();
        if (_skillComponent == null)
        {
            Debug.LogError($"[EnemyAttackAction] {Agent.Value.name}에 SkillComponent가 없습니다.");
            return Status.Failure;
        }

        if (string.IsNullOrEmpty(SkillName.Value))
        {
            Debug.LogWarning($"[EnemyAttackAction] 스킬 이름이 지정되지 않았습니다.");
            return Status.Failure;
        }

        if (!_skillComponent.skills.TryGetValue(SkillName.Value, out _currentSkill))
        {
            Debug.LogError($"[EnemyAttackAction] {SkillName.Value} 스킬을 찾을 수 없습니다.");
            return Status.Failure;
        }
        
        _currentSkill.skillEndEvent.RemoveListener(OnSkillFinished);
        _currentSkill.skillEndEvent.AddListener(OnSkillFinished);

        Debug.Log($"[EnemyAttackAction] {Agent.Value.name}가 {Target.Value.name}에게 {SkillName.Value} 시전!");
        _isAttacking = true;
        _currentSkill.ForceUseSkill(Target.Value);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isAttacking) 
            return Status.Running;
            
        return Status.Success;
    }

    protected override void OnEnd()
    {
        if (_currentSkill != null)
        {
            _currentSkill.skillEndEvent.RemoveListener(OnSkillFinished);
        }
    }

    private void OnSkillFinished()
    {
        _isAttacking = false;
    }
}