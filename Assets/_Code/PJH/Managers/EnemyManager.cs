using System.Collections.Generic;
using System.Linq;
using Code.Map;
using Code.SkillSystem;
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
            
            Unit target = GetBestTarget(enemy);
            plan.Target = target;

            if (target != null && TrySelectAttackSkill(enemy, target.gameObject, out SkillSO selectedSkill))
                plan.SelectedSkill = selectedSkill;

            return plan;
        }

        public bool TrySelectAttackSkill(AbstractEnemyUnit enemy, GameObject target, out SkillSO selectedSkillSO)
        {
            selectedSkillSO = null;

            if (enemy == null || target == null || enemy.SkillCompo?.Skills == null || enemy.SkillCompo.Skills.Count == 0)
                return false;

            SkillSO bestSkill = null;
            float bestScore = float.MinValue;

            foreach (var (skillSO, skill) in enemy.SkillCompo.Skills)
            {
                if (skillSO == null || skill == null)
                    continue;

                if (!enemy.CanUseSkillOnTarget(skillSO, target))
                    continue;

                if (skill is not EnemyBaseSkill enemySkill)
                    continue;

                float score = enemySkill.EvaluateEnemyUseScore(target);
                
                if (Mathf.Approximately(score, float.MinValue))
                    continue;

                if (bestSkill != null && score < bestScore)
                    continue;

                if (bestSkill != null && Mathf.Approximately(score, bestScore) &&
                    !IsBetterSkillCandidate(skillSO, bestSkill))
                    continue;

                bestSkill = skillSO;
                bestScore = score;
            }

            selectedSkillSO = bestSkill;
            return selectedSkillSO != null;
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

        private static bool IsBetterSkillCandidate(SkillSO candidate, SkillSO current)
        {
            if (candidate == null)
                return false;

            if (current == null)
                return true;

            if (candidate.SkillDamage != current.SkillDamage)
                return candidate.SkillDamage > current.SkillDamage;

            if (candidate.SkillCost != current.SkillCost)
                return candidate.SkillCost < current.SkillCost;

            return string.CompareOrdinal(candidate.skillName, current.skillName) < 0;
        }
    }
}
