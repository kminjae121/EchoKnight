using System;
using Unity.Behavior;
using UnityEngine;

namespace Code.UnitSystem.Enemies.AI
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "CanAttackToTarget", story: "[Enemy] can attack to [Target]", category: "Conditions", id: "4a277512aaf7c426779eff612a000870")]
    public partial class CanAttackToTargetCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemyUnit> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        public override bool IsTrue()
        {
            return Enemy.Value.AttackCompo.CanAttackToTarget(Target.Value);
        }
    }
}