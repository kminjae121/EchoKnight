using System;
using System.Collections.Generic;
using Code.Map;
using Code.Navigation;
using Code.Utils;
using UnityEngine;

namespace Code.UnitSystem.Enemies.AI
{
    public sealed class EnemyRouteMap
    {
        private static readonly Vector2Int[] Dirs =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        private readonly Queue<Vector2Int> _open = new();
        private readonly Dictionary<Vector2Int, int> _costs = new();
        private readonly Dictionary<Unit, Dictionary<Vector2Int, int>> _routes = new();

        public void Build(IReadOnlyList<Unit> targets, PathBaker baker, Func<Vector2Int, bool> canEnter)
        {
            _routes.Clear();

            if (targets == null || GridMap.Instance == null)
                return;

            foreach (var target in targets)
            {
                if (target == null)
                    continue;

                Vector2Int goal = GridMap.Instance.WorldToGridPos(target.transform.position);
                _routes[target] = BuildRoute(goal, baker, canEnter);
            }
        }

        public bool TryGetCost(Unit target, Vector2Int pos, out int cost)
        {
            cost = 0;

            if (target == null)
                return false;

            if (!_routes.TryGetValue(target, out var route))
                return false;

            return route.TryGetValue(pos, out cost);
        }

        private Dictionary<Vector2Int, int> BuildRoute(Vector2Int goal, PathBaker baker, Func<Vector2Int, bool> canEnter)
        {
            _open.Clear();
            _costs.Clear();
            Add(goal, 0);

            if (CanUseBake(goal, baker))
                BuildBaked(baker, canEnter);
            else
                BuildGrid(canEnter);

            return new Dictionary<Vector2Int, int>(_costs);
        }

        private void BuildBaked(PathBaker baker, Func<Vector2Int, bool> canEnter)
        {
            while (_open.Count > 0)
            {
                Vector2Int pos = _open.Dequeue();
                int cost = _costs[pos];

                if (!baker.bakedData.GetNodeIfExist(GridCoordUtils.GridToCell(pos), out NodeData node))
                    continue;

                foreach (LinkData link in node.neighbors)
                {
                    Vector2Int next = GridCoordUtils.CellToGrid(link.endCellPos);

                    if (!IsCardinal(pos, next))
                        continue;

                    TryAdd(next, cost + 1, canEnter);
                }
            }
        }

        private void BuildGrid(Func<Vector2Int, bool> canEnter)
        {
            while (_open.Count > 0)
            {
                Vector2Int pos = _open.Dequeue();
                int cost = _costs[pos];

                foreach (Vector2Int dir in Dirs)
                {
                    Vector2Int next = pos + dir;

                    TryAdd(next, cost + 1, canEnter);
                }
            }
        }

        private void TryAdd(Vector2Int pos, int cost, Func<Vector2Int, bool> canEnter)
        {
            if (_costs.ContainsKey(pos))
                return;

            if (GridMap.Instance != null && !GridMap.Instance.IsValidPosition(pos))
                return;

            if (canEnter != null && !canEnter(pos))
                return;

            Add(pos, cost);
        }

        private void Add(Vector2Int pos, int cost)
        {
            _costs.Add(pos, cost);
            _open.Enqueue(pos);
        }

        private static bool CanUseBake(Vector2Int goal, PathBaker baker)
        {
            return baker?.bakedData != null &&
                   baker.bakedData.GetNodeIfExist(GridCoordUtils.GridToCell(goal), out _);
        }

        private static bool IsCardinal(Vector2Int from, Vector2Int to)
        {
            Vector2Int delta = to - from;
            return Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1;
        }
    }
}
