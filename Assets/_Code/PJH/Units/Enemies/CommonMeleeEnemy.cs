using Code.Map;
using Code.UnitSystem.Enemies.AI;
using UnityEngine;

namespace Code.UnitSystem.Enemies
{
    public class CommonMeleeEnemy : AbstractEnemyUnit
    {
        [SerializeField] private GameObject target;

        private GridMap _gridMap;
        
        protected override void Start()
        {
            base.Start();
            SetVariableValue(BTVars.TargetGameObject, target);
            _gridMap = GridMap.Instance;
        }
        
        [ContextMenu("Move to target")]
        private void MoveToTarget()
        {
            Vector2Int startPos = _gridMap.WorldToGridPosition(transform.position);
            Vector2Int destination = _gridMap.WorldToGridPosition(target.transform.position);
            PathMover.SetPathAndMove(startPos, destination);
        }
    }
}