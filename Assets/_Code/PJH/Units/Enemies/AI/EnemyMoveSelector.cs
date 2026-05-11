using System.Collections.Generic;
using Code.Map;
using Code.Utils;
using UnityEngine;

namespace Code.UnitSystem.Enemies.AI
{
    public sealed class EnemyMoveSelector
    {
        private const float LeaveDeadRangeBonus = 1000f;
        private const float CanUseBonus = 500f;
        private const float RetreatPosWeight = 10f;

        public bool TrySkillTile(AbstractEnemyUnit enemy, IReadOnlyList<EnemyMoveOption> options, out EnemyMovePick move)
        {
            move = default;

            if (enemy == null || options == null)
                return false;

            var best = default(EnemyMoveOption);
            var found = false;

            foreach (var option in options)
            {
                if (!option.IsValid)
                    continue;

                if (found && option.SkillScore < best.SkillScore)
                    continue;

                if (found && Mathf.Approximately(option.SkillScore, best.SkillScore))
                {
                    if (option.PosScore < best.PosScore)
                        continue;

                    if (Mathf.Approximately(option.PosScore, best.PosScore))
                    {
                        if (option.Cost > best.Cost)
                            continue;

                        if (option.Cost == best.Cost && IsWorseDistance(enemy, option.Distance, best.Distance))
                            continue;
                    }
                }

                best = option;
                found = true;
            }

            if (!found)
                return false;

            move = new EnemyMovePick(best.Target, best.Tile);
            return true;
        }

        public bool TrySpaceTile(AbstractEnemyUnit enemy, Vector2Int from, EnemySkillPick pick, IReadOnlyList<Vector2Int> tiles, out Vector2Int selectedTile)
        {
            selectedTile = default;

            if (enemy == null || !pick.IsValid || tiles == null || GridMap.Instance == null)
                return false;

            Vector2Int targetPos = GridMap.Instance.WorldToGridPos(pick.Target.transform.position);
            var bestScore = float.MinValue;
            var bestCost = int.MaxValue;
            var bestDistance = float.MinValue;
            var found = false;

            foreach (var tile in tiles)
            {
                if (tile == from)
                    continue;

                var target = pick.Target.gameObject;

                if (!pick.Skill.CanUseAt(tile, target))
                    continue;

                if (pick.Skill.WantsMove(tile, target))
                    continue;

                float score = pick.Skill.PosScore(tile, target);
                int cost = GetCost(from, tile);
                float distance = DistanceUtils.GetChebyshevDistance(tile, targetPos);

                if (found && score < bestScore)
                    continue;

                if (found && Mathf.Approximately(score, bestScore))
                {
                    if (cost > bestCost)
                        continue;

                    if (cost == bestCost && distance <= bestDistance)
                        continue;
                }

                selectedTile = tile;
                bestScore = score;
                bestCost = cost;
                bestDistance = distance;
                found = true;
            }

            return found;
        }

        public bool TryRetreatTile(AbstractEnemyUnit enemy, Vector2Int from, EnemySkillPick pick, IReadOnlyList<Vector2Int> tiles, out Vector2Int selectedTile)
        {
            selectedTile = default;

            if (enemy == null || !pick.IsValid || tiles == null || GridMap.Instance == null)
                return false;

            var target = pick.Target.gameObject;
            Vector2Int targetPos = GridMap.Instance.WorldToGridPos(pick.Target.transform.position);
            float currentDistance = DistanceUtils.GetChebyshevDistance(from, targetPos);
            var bestScore = float.MinValue;
            var bestCost = int.MaxValue;
            var found = false;

            foreach (var tile in tiles)
            {
                if (tile == from)
                    continue;

                float distance = DistanceUtils.GetChebyshevDistance(tile, targetPos);

                if (distance <= currentDistance)
                    continue;

                int cost = GetCost(from, tile);
                float score = distance;

                if (!pick.Skill.TooClose(tile, target))
                    score += LeaveDeadRangeBonus;

                if (pick.Skill.CanUseAt(tile, target))
                    score += CanUseBonus;

                score += pick.Skill.PosScore(tile, target) * RetreatPosWeight;

                if (found && score < bestScore)
                    continue;

                if (found && Mathf.Approximately(score, bestScore) && cost >= bestCost)
                    continue;

                selectedTile = tile;
                bestScore = score;
                bestCost = cost;
                found = true;
            }

            return found;
        }

        public bool TryApproachTile(Vector2Int from, Vector2Int targetPos, IReadOnlyList<Vector2Int> tiles, out Vector2Int selectedTile)
        {
            selectedTile = default;

            if (tiles == null)
                return false;

            float currentDistance = DistanceUtils.GetChebyshevDistance(from, targetPos);
            float bestDistance = currentDistance;
            var bestCost = int.MaxValue;
            var found = false;

            foreach (var tile in tiles)
            {
                if (tile == from)
                    continue;

                float distance = DistanceUtils.GetChebyshevDistance(tile, targetPos);
                int cost = GetCost(from, tile);

                if (distance > bestDistance)
                    continue;

                if (Mathf.Approximately(distance, bestDistance) && cost >= bestCost)
                    continue;

                bestDistance = distance;
                bestCost = cost;
                selectedTile = tile;
                found = true;
            }

            return found;
        }

        public static int GetCost(Vector2Int from, Vector2Int to)
        {
            Vector2Int delta = to - from;
            return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        }

        private static bool IsWorseDistance(AbstractEnemyUnit enemy, float candidateDistance, float currentDistance)
        {
            if (enemy?.AIProfile != null && enemy.AIProfile.WantsSpace)
                return candidateDistance <= currentDistance;

            return candidateDistance >= currentDistance;
        }
    }
}