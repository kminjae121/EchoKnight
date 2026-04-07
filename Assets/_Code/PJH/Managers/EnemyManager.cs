using System.Collections.Generic;
using System.Linq;
using Code.Map;
using Code.UnitSystem;
using Code.UnitSystem.Enemies;
using Code.UnitSystem.Enemies.AI;
using Code.Utils;
using GondrLib.Dependencies;
using UnityEngine;

namespace Code.Managers
{
    [Provide]
    public class EnemyManager : MonoBehaviour, IDependencyProvider
    {
        [Inject] private UnitManager _unitManager;

        private readonly Dictionary<AbstractEnemyUnit, EnemyPlan> _plans = new();
        private readonly Dictionary<Vector2Int, AbstractEnemyUnit> _reservedTiles = new();

        public EnemyPlan BuildPlan(AbstractEnemyUnit enemy)
        {
            if (enemy == null)
                return null;

            EnemyPlan plan = GetOrCreatePlan(enemy);
            plan.Clear();
            plan.SetTarget(GetBestTarget(enemy));
            return plan;
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

            if (_reservedTiles.TryGetValue(tilePos, out AbstractEnemyUnit reservedEnemy) && reservedEnemy != enemy)
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
            if (_plans.TryGetValue(enemy, out EnemyPlan plan))
                return plan;

            plan = new EnemyPlan();
            _plans.Add(enemy, plan);
            return plan;
        }

        private Unit GetBestTarget(AbstractEnemyUnit enemy)
        {
            if (enemy == null || _unitManager == null || GridMap.Instance == null)
                return null;

            Vector2Int myPos = GridMap.Instance.WorldToGridPosition(enemy.transform.position);

            return _unitManager.GetPlayerUnits()
                .Where(unit => unit != null && unit.gameObject.activeInHierarchy)
                .OrderBy(unit => DistanceUtils.GetEuclideanDistance(myPos,
                    GridMap.Instance.WorldToGridPosition(unit.transform.position)))
                .FirstOrDefault();
        }
    }
}
