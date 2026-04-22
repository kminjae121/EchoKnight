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
        [SerializeField] private UnitManager unitManager;

        private readonly Dictionary<AbstractEnemyUnit, EnemyPlan> _plans = new();
        private readonly Dictionary<Vector2Int, AbstractEnemyUnit> _reservedTiles = new();

        public void RefreshPlan(AbstractEnemyUnit enemy)
        {
            if (enemy == null)
                return;

            EnemyPlan plan = GetOrCreatePlan(enemy);
            plan.ClearCombatDecision();

            Unit fallbackTarget = GetClosestTarget(enemy);

            if (TrySelectBestCombatOption(enemy, out Unit selectedTarget, out SkillSO selectedSkill))
            {
                plan.SetTarget(selectedTarget);
                plan.SetSkill(selectedSkill);
                return;
            }

            if (fallbackTarget != null)
                plan.SetTarget(fallbackTarget);
        }

        private bool TrySelectBestCombatOption(AbstractEnemyUnit enemy, out Unit selectedTarget, out SkillSO selectedSkillSO)
        {
            selectedTarget = null;
            selectedSkillSO = null;

            if (enemy == null || unitManager == null || GridMap.Instance == null)
                return false;

            Vector2Int myPos = GridMap.Instance.WorldToGridPos(enemy.transform.position);
            SkillSO bestSkill = null;
            Unit bestTarget = null;
            float bestScore = float.MinValue;
            float bestDistance = float.MaxValue;

            foreach (Unit target in GetCandidateTargets())
            {
                if (!TrySelectBestSkillForTarget(enemy, target.gameObject, out SkillSO candidateSkill, out float candidateScore))
                    continue;

                float candidateDistance = DistanceUtils.GetEuclideanDistance(myPos,
                    GridMap.Instance.WorldToGridPos(target.transform.position));

                if (bestTarget != null && candidateScore < bestScore)
                    continue;

                if (bestTarget != null && Mathf.Approximately(candidateScore, bestScore))
                {
                    if (candidateDistance > bestDistance)
                        continue;

                    if (Mathf.Approximately(candidateDistance, bestDistance) &&
                        !IsBetterSkillCandidate(candidateSkill, bestSkill))
                        continue;
                }

                bestTarget = target;
                bestSkill = candidateSkill;
                bestScore = candidateScore;
                bestDistance = candidateDistance;
            }

            selectedTarget = bestTarget;
            selectedSkillSO = bestSkill;
            return selectedTarget != null && selectedSkillSO != null;
        }

        private bool TrySelectBestSkillForTarget(AbstractEnemyUnit enemy, GameObject target, out SkillSO selectedSkillSO, out float selectedScore)
        {
            selectedSkillSO = null;
            selectedScore = float.MinValue;

            if (enemy == null || target == null || enemy.SkillCompo?.Skills == null || enemy.SkillCompo.Skills.Count == 0)
                return false;

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

                if (selectedSkillSO != null && score < selectedScore)
                    continue;

                if (selectedSkillSO != null && Mathf.Approximately(score, selectedScore) &&
                    !IsBetterSkillCandidate(skillSO, selectedSkillSO))
                    continue;

                selectedSkillSO = skillSO;
                selectedScore = score;
            }

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

        private Unit GetClosestTarget(AbstractEnemyUnit enemy)
        {
            if (enemy == null || unitManager == null || GridMap.Instance == null)
                return null;

            Vector2Int myPos = GridMap.Instance.WorldToGridPos(enemy.transform.position);

            return GetCandidateTargets()
                .OrderBy(unit => DistanceUtils.GetEuclideanDistance(myPos,
                    GridMap.Instance.WorldToGridPos(unit.transform.position)))
                .FirstOrDefault();
        }

        private IEnumerable<Unit> GetCandidateTargets()
        {
            if (unitManager == null)
                return Enumerable.Empty<Unit>();

            return unitManager.GetPlayerUnits()
                .Where(unit => unit != null && unit.gameObject.activeInHierarchy);
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
