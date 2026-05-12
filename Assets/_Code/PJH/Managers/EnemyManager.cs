using System.Collections.Generic;
using System.Linq;
using Code.Map;
using Code.UnitSystem;
using Code.UnitSystem.Enemies;
using Code.UnitSystem.Enemies.AI;
using GondrLib.Dependencies;
using UnityEngine;

namespace Code.Managers
{
    [Provide]
    public class EnemyManager : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private UnitManager unitManager;

        private readonly Dictionary<AbstractEnemyUnit, EnemyPlan> _plans = new();
        private readonly Dictionary<Vector2Int, AbstractEnemyUnit> _reservedTiles = new();
        private readonly EnemyPlanner _planner = new();

        public void RefreshPlan(AbstractEnemyUnit enemy)
        {
            if (enemy == null)
                return;

            EnemyPlan plan = GetOrCreatePlan(enemy);
            plan.Clear();

            var gridMap = GridMap.Instance;

            if (unitManager == null || gridMap == null)
                return;

            Vector2Int currentPos = gridMap.WorldToGridPos(enemy.transform.position);
            List<Unit> targets = GetTargets();
            List<Vector2Int> tiles = GetTiles(enemy, currentPos);

            _planner.Build(plan, enemy, currentPos, targets, tiles);
        }

        public bool TryGetPlan(AbstractEnemyUnit enemy, out EnemyPlan plan)
        {
            if (enemy == null)
            {
                plan = null;
                return false;
            }

            return _plans.TryGetValue(enemy, out plan);
        }

        public bool TryReserveTile(AbstractEnemyUnit enemy, Vector2Int tilePos)
        {
            if (enemy == null)
                return false;

            if (_reservedTiles.TryGetValue(tilePos, out var reservedEnemy) && reservedEnemy != enemy)
                return false;

            ReleaseReservation(enemy);
            _reservedTiles[tilePos] = enemy;
            return true;
        }

        public void ReleaseReservation(AbstractEnemyUnit enemy)
        {
            if (enemy == null)
                return;

            Vector2Int releaseKey = default;
            bool found = false;

            foreach (var pair in _reservedTiles)
            {
                if (pair.Value != enemy)
                    continue;

                releaseKey = pair.Key;
                found = true;
                break;
            }

            if (found)
                _reservedTiles.Remove(releaseKey);
        }

        public void RemovePlan(AbstractEnemyUnit enemy)
        {
            if (enemy == null)
                return;

            ReleaseReservation(enemy);
            _plans.Remove(enemy);
        }

        public void ClearTurnReservations()
            => _reservedTiles.Clear();

        private EnemyPlan GetOrCreatePlan(AbstractEnemyUnit enemy)
        {
            if (_plans.TryGetValue(enemy, out var plan))
                return plan;

            plan = new EnemyPlan();
            _plans.Add(enemy, plan);
            return plan;
        }

        private List<Unit> GetTargets()
        {
            if (unitManager == null)
                return new List<Unit>();

            return unitManager.GetPlayerUnits()
                .Where(unit => unit != null && unit.gameObject.activeInHierarchy)
                .ToList();
        }

        private List<Vector2Int> GetTiles(AbstractEnemyUnit enemy, Vector2Int currentPos)
        {
            var tiles = new List<Vector2Int>();
            var gridMap = GridMap.Instance;

            if (enemy == null || gridMap == null)
                return tiles;

            int moveRange = GetMoveRange(enemy);

            if (moveRange <= 0)
                return tiles;

            for (int y = currentPos.y - moveRange; y <= currentPos.y + moveRange; ++y)
            {
                for (int x = currentPos.x - moveRange; x <= currentPos.x + moveRange; ++x)
                {
                    var tile = new Vector2Int(x, y);

                    if (!gridMap.IsValidPosition(tile))
                        continue;

                    if (EnemyMoveSelector.GetCost(currentPos, tile) > moveRange)
                        continue;

                    if (!CanMoveTo(enemy, currentPos, tile))
                        continue;

                    tiles.Add(tile);
                }
            }

            return tiles;
        }

        private static int GetMoveRange(AbstractEnemyUnit enemy)
            => enemy?.unitSO == null ? 0 : Mathf.Max(0, enemy.unitSO.MoveRange);

        private bool CanMoveTo(AbstractEnemyUnit enemy, Vector2Int currentPos, Vector2Int tile)
        {
            if (tile == currentPos)
                return true;

            if (!GridMap.Instance.CanMoveTo(tile))
                return false;

            return !_reservedTiles.TryGetValue(tile, out var reservedEnemy) || reservedEnemy == enemy;
        }
    }
}