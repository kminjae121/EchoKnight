using System.Linq;
using Code.Managers;
using Code.Map;
using Code.UnitSystem.Enemies.AI;
using Code.Utils;
using UnityEngine;

namespace Code.UnitSystem.Enemies
{
    public class CommonMeleeEnemy : AbstractEnemyUnit
    {
        private GridMap _gridMap;
        private UnitManager _unitManager;
        private Unit _currentTarget;

        protected override void Start()
        {
            base.Start();
            _gridMap = GridMap.Instance;
            _unitManager = FindFirstObjectByType<UnitManager>();
            UpdateTargetBlackboard();
        }

        protected override bool PrepareTurnStart()
        {
            return UpdateTargetBlackboard();
        }

        [ContextMenu("Move to target")]
        private void MoveToTarget()
        {
            if (_gridMap == null || _currentTarget == null)
                return;

            Vector2Int startPos = _gridMap.WorldToGridPosition(transform.position);
            Vector2Int destination = _gridMap.WorldToGridPosition(_currentTarget.transform.position);
            PathMover.SetPathAndMove(startPos, destination);
        }

        private bool UpdateTargetBlackboard()
        {
            if (_gridMap == null || _unitManager == null)
            {
                _currentTarget = null;
                SetVariableValue<GameObject>(BTVars.Target, null);
                return false;
            }

            Vector2Int myPos = _gridMap.WorldToGridPosition(transform.position);

            _currentTarget = _unitManager.GetPlayerUnits()
                .Where(unit => unit != null && unit.gameObject.activeInHierarchy)
                .OrderBy(unit => DistanceUtils.GetEuclideanDistance(myPos, _gridMap.WorldToGridPosition(unit.transform.position)))
                .FirstOrDefault();

            SetVariableValue(BTVars.Target, _currentTarget != null ? _currentTarget.gameObject : null);

            return _currentTarget != null;
        }
    }
}
