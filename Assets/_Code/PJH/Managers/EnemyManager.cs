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
            plan.Clear();

            if (unitManager == null || GridMap.Instance == null)
                return;

            Vector2Int currentPos = GridMap.Instance.WorldToGridPos(enemy.transform.position);

            bool hasCombatOption = TrySelectBestCombatOption(enemy, currentPos, out Unit selectedTarget,
                out SkillSO selectedSkill, out EnemyBaseSkill selectedEnemySkill);
            bool shouldReposition = hasCombatOption &&
                                    selectedEnemySkill != null &&
                                    selectedEnemySkill.ShouldPreferRepositionFromPosition(currentPos,
                                        selectedTarget.gameObject);

            if (hasCombatOption && !shouldReposition)
            {
                plan.SetCombatDecision(selectedTarget, selectedSkill);
                return;
            }

            if (TrySelectBestMoveOption(enemy, currentPos, out Unit moveTarget, out Vector2Int moveTile))
            {
                plan.SetTarget(moveTarget);
                plan.SetMoveTile(moveTile);
                return;
            }

            if (hasCombatOption)
            {
                plan.SetCombatDecision(selectedTarget, selectedSkill);
                return;
            }

            Unit fallbackTarget = GetClosestTarget(enemy);

            if (fallbackTarget == null)
                return;

            plan.SetTarget(fallbackTarget);

            if (TryGetBestApproachTile(enemy, currentPos,
                    GridMap.Instance.WorldToGridPos(fallbackTarget.transform.position), out Vector2Int approachTile))
                plan.SetMoveTile(approachTile);
        }

        private bool TrySelectBestCombatOption(AbstractEnemyUnit enemy, Vector2Int sourcePos, out Unit selectedTarget,
            out SkillSO selectedSkillSO, out EnemyBaseSkill selectedEnemySkill)
        {
            selectedTarget = null;
            selectedSkillSO = null;
            selectedEnemySkill = null;

            if (enemy == null || unitManager == null || GridMap.Instance == null)
                return false;

            SkillSO bestSkill = null;
            EnemyBaseSkill bestEnemySkill = null;
            Unit bestTarget = null;
            float bestScore = float.MinValue;
            float bestDistance = float.MaxValue;

            foreach (var target in GetCandidateTargets())
            {
                if (!TrySelectBestSkillForTarget(enemy, sourcePos, target.gameObject,
                        out SkillSO candidateSkill, out EnemyBaseSkill candidateEnemySkill, out float candidateScore))
                    continue;

                float candidateDistance = DistanceUtils.GetEuclideanDistance(sourcePos,
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
                bestEnemySkill = candidateEnemySkill;
                bestScore = candidateScore;
                bestDistance = candidateDistance;
            }

            selectedTarget = bestTarget;
            selectedSkillSO = bestSkill;
            selectedEnemySkill = bestEnemySkill;
            return selectedTarget != null && selectedSkillSO != null;
        }

        private bool TrySelectBestSkillForTarget(AbstractEnemyUnit enemy, Vector2Int sourcePos, GameObject target,
            out SkillSO selectedSkillSO, out EnemyBaseSkill selectedEnemySkill, out float selectedScore)
        {
            selectedSkillSO = null;
            selectedEnemySkill = null;
            selectedScore = float.MinValue;

            if (enemy == null || target == null || enemy.SkillCompo?.Skills == null || enemy.SkillCompo.Skills.Count == 0)
                return false;

            foreach (var (skillSO, skill) in enemy.SkillCompo.Skills)
            {
                if (skillSO == null || skill == null)
                    continue;

                if (skill is not EnemyBaseSkill enemySkill)
                    continue;

                if (!enemySkill.CanUseOnTargetFromPosition(sourcePos, target))
                    continue;

                float score = enemySkill.EvaluateEnemyUseScoreFromPosition(sourcePos, target);
                
                if (Mathf.Approximately(score, float.MinValue))
                    continue;

                if (selectedSkillSO != null && score < selectedScore)
                    continue;

                if (selectedSkillSO != null && Mathf.Approximately(score, selectedScore) &&
                    !IsBetterSkillCandidate(skillSO, selectedSkillSO))
                    continue;

                selectedSkillSO = skillSO;
                selectedEnemySkill = enemySkill;
                selectedScore = score;
            }

            return selectedSkillSO != null;
        }

        private bool TrySelectBestMoveOption(AbstractEnemyUnit enemy, Vector2Int currentPos, out Unit selectedTarget, out Vector2Int selectedMoveTile)
        {
            selectedTarget = null;
            selectedMoveTile = default;

            if (enemy == null || GridMap.Instance == null)
                return false;

            int moveRange = GetMoveRange(enemy);

            if (moveRange <= 0)
                return false;

            Unit bestTarget = null;
            Vector2Int bestMoveTile = default;
            float bestScore = float.MinValue;
            float bestPositionScore = float.MinValue;
            int bestMoveCost = int.MaxValue;
            float bestTargetDistance = float.MaxValue;

            foreach (Unit target in GetCandidateTargets())
            {
                if (target == null)
                    continue;

                Vector2Int targetPos = GridMap.Instance.WorldToGridPos(target.transform.position);

                foreach (Vector2Int candidateTile in GetCandidateMoveTiles(enemy, currentPos, moveRange))
                {
                    if (candidateTile == currentPos)
                        continue;

                    if (!TrySelectBestSkillForTarget(enemy, candidateTile, target.gameObject,
                            out _, out EnemyBaseSkill selectedEnemySkill, out float score))
                        continue;

                    int moveCost = GetMoveCost(currentPos, candidateTile);
                    float targetDistance = DistanceUtils.GetEuclideanDistance(candidateTile, targetPos);
                    float positionScore = selectedEnemySkill?.EvaluatePositionPreferenceScoreFromPosition(candidateTile, target.gameObject) ?? 0f;

                    if (bestTarget != null && score < bestScore)
                        continue;

                    if (bestTarget != null && Mathf.Approximately(score, bestScore))
                    {
                        if (positionScore < bestPositionScore)
                            continue;

                        if (Mathf.Approximately(positionScore, bestPositionScore))
                        {
                            if (moveCost > bestMoveCost)
                                continue;

                            if (moveCost == bestMoveCost && targetDistance >= bestTargetDistance)
                                continue;
                        }
                    }

                    bestTarget = target;
                    bestMoveTile = candidateTile;
                    bestScore = score;
                    bestPositionScore = positionScore;
                    bestMoveCost = moveCost;
                    bestTargetDistance = targetDistance;
                }
            }

            selectedTarget = bestTarget;
            selectedMoveTile = bestMoveTile;
            return selectedTarget != null;
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

        private bool TryGetBestApproachTile(AbstractEnemyUnit enemy, Vector2Int currentPos, Vector2Int targetPos, out Vector2Int moveTile)
        {
            moveTile = default;

            if (enemy == null || GridMap.Instance == null)
                return false;

            int moveRange = GetMoveRange(enemy);

            if (moveRange <= 0)
                return false;

            float currentDistance = DistanceUtils.GetEuclideanDistance(currentPos, targetPos);
            float bestDistance = currentDistance;
            int bestMoveCost = int.MaxValue;
            bool found = false;

            foreach (Vector2Int candidateTile in GetCandidateMoveTiles(enemy, currentPos, moveRange))
            {
                if (candidateTile == currentPos)
                    continue;

                float candidateDistance = DistanceUtils.GetEuclideanDistance(candidateTile, targetPos);
                int moveCost = GetMoveCost(currentPos, candidateTile);

                if (candidateDistance > bestDistance)
                    continue;

                if (Mathf.Approximately(candidateDistance, bestDistance) && moveCost >= bestMoveCost)
                    continue;

                bestDistance = candidateDistance;
                bestMoveCost = moveCost;
                moveTile = candidateTile;
                found = true;
            }

            return found;
        }

        private IEnumerable<Unit> GetCandidateTargets()
        {
            if (unitManager == null)
                return Enumerable.Empty<Unit>();

            return unitManager.GetPlayerUnits()
                .Where(unit => unit != null && unit.gameObject.activeInHierarchy);
        }

        private IEnumerable<Vector2Int> GetCandidateMoveTiles(AbstractEnemyUnit enemy, Vector2Int currentPos, int moveRange)
        {
            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                yield break;

            for (int y = currentPos.y - moveRange; y <= currentPos.y + moveRange; ++y)
                for (int x = currentPos.x - moveRange; x <= currentPos.x + moveRange; ++x)
                {
                    Vector2Int candidateTile = new Vector2Int(x, y);

                    if (!gridMap.IsValidPosition(candidateTile))
                        continue;

                    if (GetMoveCost(currentPos, candidateTile) > moveRange)
                        continue;

                    if (!CanMoveToCandidate(enemy, currentPos, candidateTile))
                        continue;

                    yield return candidateTile;
                }
        }

        private static int GetMoveRange(AbstractEnemyUnit enemy)
        {
            if (enemy?.unitSO == null)
                return 0;

            return Mathf.Max(0, enemy.unitSO.MoveRange);
        }

        private bool CanMoveToCandidate(AbstractEnemyUnit enemy, Vector2Int currentPos, Vector2Int candidateTile)
        {
            if (candidateTile == currentPos)
                return true;

            if (!GridMap.Instance.CanMoveTo(candidateTile))
                return false;

            if (_reservedTiles.TryGetValue(candidateTile, out AbstractEnemyUnit reservedEnemy) && reservedEnemy != enemy)
                return false;

            return true;
        }

        private static int GetMoveCost(Vector2Int currentPos, Vector2Int candidateTile)
        {
            Vector2Int delta = candidateTile - currentPos;
            return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
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
