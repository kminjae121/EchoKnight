using Code.SkillSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Code.UnitSystem.Enemies.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "UseSkill", story: "[Enemy] use skill [SkillData] to [Target]", category: "Action", id: "783da2698f5bd1e0e9055ff7658a1dd8")]
    public partial class UseSkillAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemyUnit> Enemy;
        [SerializeReference] public BlackboardVariable<SkillSO> SkillData;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private bool _isAttacking;
    
        protected override Status OnStart()
        {
            if (Enemy.Value == null || SkillData.Value == null || Target.Value == null)
                return Status.Failure;

            _isAttacking = true;
            Enemy.Value.OrderSkill(SkillData.Value, Target.Value, HandleAttackEnd);
        
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _isAttacking ? Status.Running : Status.Success;
        }

        protected override void OnEnd()
        {
            _isAttacking = false;
        }
    
        private void HandleAttackEnd()
            =>_isAttacking = false;
    }
}
