using System.Collections.Generic;
using Code.Map;
using Code.Utils;
using UnityEngine;

namespace Code.UnitSystem.Enemies.AI
{
    public sealed class EnemyPlanner
    {
        private readonly EnemySkillSelector _skills = new();
        private readonly EnemyMoveSelector _moves = new();

        public void Build(EnemyPlan plan, AbstractEnemyUnit enemy, Vector2Int from, IReadOnlyList<Unit> targets, IReadOnlyList<Vector2Int> tiles)
        {
            if (plan == null || enemy == null || targets == null || targets.Count == 0)
                return;

            bool hasPick = _skills.TryBest(enemy, from, targets, out EnemySkillPick pick);
            bool canKeepSpace = CanKeepSpace(enemy);
            bool wantsMove = hasPick && canKeepSpace && pick.Skill.WantsMove(from, pick.Target.gameObject);

            if (hasPick && !wantsMove)
            {
                plan.SetCombatDecision(pick.Target, pick.SkillSO);
                return;
            }

            if (hasPick && _moves.TrySpaceTile(enemy, from, pick, tiles, out Vector2Int spaceTile))
            {
                plan.SetTarget(pick.Target);
                plan.SetMoveTile(spaceTile);
                return;
            }

            if (wantsMove && _moves.TryRetreatTile(enemy, from, pick, tiles, out Vector2Int retreatTile))
            {
                plan.SetTarget(pick.Target);
                plan.SetMoveTile(retreatTile);
                return;
            }

            if (hasPick && (enemy.AIProfile == null || enemy.AIProfile.AttackCornered))
            {
                plan.SetCombatDecision(pick.Target, pick.SkillSO);
                return;
            }

            if (!hasPick && canKeepSpace && _skills.TryTooClose(enemy, from, targets, out EnemySkillPick closePick))
            {
                plan.SetTarget(closePick.Target);

                if (_moves.TryRetreatTile(enemy, from, closePick, tiles, out Vector2Int closeRetreatTile))
                    plan.SetMoveTile(closeRetreatTile);

                return;
            }

            List<EnemyMoveOption> skillTiles = BuildSkillTileOptions(enemy, from, targets, tiles);

            if (_moves.TrySkillTile(enemy, skillTiles, out EnemyMovePick move))
            {
                plan.SetTarget(move.Target);
                plan.SetMoveTile(move.Tile);
                return;
            }

            Unit target = PickClosest(from, targets);

            if (target == null)
                return;

            if (_skills.IsTooClose(enemy, from, target.gameObject))
            {
                plan.SetTarget(target);
                return;
            }

            plan.SetTarget(target);

            if (GridMap.Instance == null)
                return;

            Vector2Int targetPos = GridMap.Instance.WorldToGridPos(target.transform.position);

            if (_moves.TryApproachTile(from, targetPos, tiles, out Vector2Int approachTile))
                plan.SetMoveTile(approachTile);
        }

        private static Unit PickClosest(Vector2Int from, IReadOnlyList<Unit> targets)
        {
            if (targets == null || GridMap.Instance == null)
                return null;

            Unit closest = null;
            var bestDistance = float.MaxValue;

            foreach (var target in targets)
            {
                if (target == null)
                    continue;

                float distance = DistanceUtils.GetEuclideanDistance(from,
                    GridMap.Instance.WorldToGridPos(target.transform.position));

                if (closest != null && distance >= bestDistance)
                    continue;

                closest = target;
                bestDistance = distance;
            }

            return closest;
        }

        private List<EnemyMoveOption> BuildSkillTileOptions(AbstractEnemyUnit enemy, Vector2Int from, IReadOnlyList<Unit> targets, IReadOnlyList<Vector2Int> tiles)
        {
            var options = new List<EnemyMoveOption>();
            var gridMap = GridMap.Instance;

            if (enemy == null || targets == null || tiles == null || gridMap == null)
                return options;

            foreach (var target in targets)
            {
                if (target == null)
                    continue;

                Vector2Int targetPos = gridMap.WorldToGridPos(target.transform.position);

                foreach (var tile in tiles)
                {
                    if (tile == from)
                        continue;

                    if (!_skills.TrySkill(enemy, tile, target.gameObject, out EnemySkillPick pick))
                        continue;

                    options.Add(new EnemyMoveOption(
                        target,
                        tile,
                        pick.Score,
                        pick.Skill.PosScore(tile, target.gameObject),
                        EnemyMoveSelector.GetCost(from, tile),
                        DistanceUtils.GetChebyshevDistance(tile, targetPos)));
                }
            }

            return options;
        }

        private static bool CanKeepSpace(AbstractEnemyUnit enemy)
            => enemy?.AIProfile == null || enemy.AIProfile.WantsSpace;
    }
}