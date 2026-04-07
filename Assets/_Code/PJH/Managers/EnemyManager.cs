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
            plan.SetTarget(target);

            if (target != null && TrySelectAttackSkill(enemy, target.gameObject, out SkillSO selectedSkill))
                plan.SetSkill(selectedSkill);

            return plan;
        }

        public bool TrySelectAttackSkill(AbstractEnemyUnit enemy, GameObject target, out SkillSO selectedSkillSO)
        {
            selectedSkillSO = null;

            if (enemy == null || target == null || enemy.SkillCompo?.Skills == null || enemy.SkillCompo.Skills.Count == 0)
                return false;

            SkillSO bestPierceSkill = null;
            int bestPierceHitCount = 0;
            SkillSO basicSkill = null;
            SkillSO fallbackSkill = null;

            foreach (var (skillSO, skill) in enemy.SkillCompo.Skills)
            {
                if (skillSO == null || skill == null)
                    continue;

                if (!enemy.CanUseSkillOnTarget(skillSO, target))
                    continue;

                fallbackSkill ??= skillSO;

                if (skill is FrontPierceEnemyAttack pierceSkill)
                {
                    int hitCount = pierceSkill.GetPredictedHitCount(target);

                    if (hitCount > bestPierceHitCount)
                    {
                        bestPierceHitCount = hitCount;
                        bestPierceSkill = skillSO;
                    }

                    continue;
                }

                if (skillSO.SkillType == SkillType.BasicSkill)
                    basicSkill ??= skillSO;
            }

            if (bestPierceSkill != null && bestPierceHitCount >= 2)
            {
                selectedSkillSO = bestPierceSkill;
                return true;
            }

            if (basicSkill != null)
            {
                selectedSkillSO = basicSkill;
                return true;
            }

            if (bestPierceSkill != null)
            {
                selectedSkillSO = bestPierceSkill;
                return true;
            }

            if (fallbackSkill != null)
            {
                selectedSkillSO = fallbackSkill;
                return true;
            }

            return false;
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
