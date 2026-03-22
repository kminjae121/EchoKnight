using Code.UnitSystem.Enemies.AI;
using UnityEngine;

namespace Code.UnitSystem.Enemies
{
    public class CommonMeleeEnemy : AbstractEnemyUnit
    {
        [SerializeField] private GameObject target;

        protected override void Start()
        {
            base.Start();
            SetVariableValue(BTVars.TargetGameObject, target);
        }
        
        [ContextMenu("Move to target")]
        private void MoveToTarget()
        {
        }
    }
}