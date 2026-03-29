using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.UnitSystem.Enemies.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Attack", story: "[Enemy] attack to [Target]", category: "Action", id: "28e92bdac50d0e2b049b663a735b8e90")]
    public partial class AttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemyUnit> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private TestAttackCompo _attackCompo;
        private bool _isAttacking;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Target.Value == null)
                return Status.Failure;

            if (_attackCompo == null)
                return Status.Failure;

            _attackCompo.OnAttackEnd += HandleAttackEnd;
            _isAttacking = true;
            _attackCompo.Attack(Target.Value);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _isAttacking ? Status.Running : Status.Success;
        }

        protected override void OnEnd()
        {
            if (_attackCompo != null)
                _attackCompo.OnAttackEnd -= HandleAttackEnd;
        }

        private void HandleAttackEnd()
        {
            _isAttacking = false;
        }
    }
}