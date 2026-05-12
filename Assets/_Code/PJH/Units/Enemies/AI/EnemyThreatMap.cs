using System.Collections.Generic;
using Code.Map;
using Code.Utils;
using UnityEngine;

namespace Code.UnitSystem.Enemies.AI
{
    public sealed class EnemyThreatMap
    {
        private readonly List<Vector2Int> _positions = new();

        public EnemyThreatMap(IReadOnlyList<Unit> units)
        {
            if (units == null || GridMap.Instance == null)
                return;

            foreach (Unit unit in units)
            {
                if (unit == null)
                    continue;

                _positions.Add(GridMap.Instance.WorldToGridPos(unit.transform.position));
            }
        }

        public float MinDist(Vector2Int from, float fallback)
        {
            if (_positions.Count == 0)
                return fallback;

            float min = float.MaxValue;

            foreach (Vector2Int pos in _positions)
            {
                float dist = DistanceUtils.GetManhattanDistance(from, pos);

                if (dist < min)
                    min = dist;
            }

            return min;
        }
    }
}
