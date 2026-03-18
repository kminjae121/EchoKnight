using System;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Map;
using UnityEngine;

namespace Code.UnitSystem
{
    public class RangeComponent : MonoBehaviour, IUnitComponent
    {
        public bool IsActive { get; set; }
        public bool isMove;

        protected Action _resetTileEvent;
        protected Unit _owner;

        protected readonly List<IMapTile> _tilesInRange = new();

        private UnitManageRangeCompo _rangeComponent;

        public void Initialize(Unit owner)
        {
            _owner = owner;
            _rangeComponent = owner.GetUnitCompo<UnitManageRangeCompo>();
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        public void FindObjectInRange()
        {
            _rangeComponent.RemoveAllRange(); 

            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));

            CalculateRange();
            ProcessTiles(_tilesInRange, true);

            IsActive = true;
        }

        private void CalculateRange()
        {
            _tilesInRange.Clear();
            
            Vector2Int start = GridMap.Instance.WorldToGridPosition(transform.position);
            int range = _owner.unitSO.moveRange;

            Queue<(Vector2Int pos, int dist)> queue = new();
            HashSet<Vector2Int> visited = new();

            queue.Enqueue((start, 0));
            visited.Add(start);

            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            while (queue.Count > 0)
            {
                var (pos, dist) = queue.Dequeue();

                if (dist >= range)
                    continue;

                foreach (var dir in dirs)
                {
                    Vector2Int next = pos + dir;

                    if (visited.Contains(next))
                        continue;

                    IMapTile tile = GridMap.Instance.GetTile(next);

                    if (tile == null)
                        continue;

                    visited.Add(next);
                    
                    if (tile.HasObstacle)
                        continue;
                    
                    _tilesInRange.Add(tile);
                    queue.Enqueue((next, dist + 1));
                }
            }
        }

        public void ResetTile()
        {
            if (_tilesInRange.Count == 0)
                return;

            ProcessTiles(_tilesInRange, false);

            IsActive = false;

            _resetTileEvent?.Invoke();
        }

        public void ReCheckInRange()
        {
            foreach (var tile in _tilesInRange)
                if (!tile.HasObstacle)
                    tile.SetWalkable(true);

            IsActive = true;
        }

        public void EndAct()
        {
            IsActive = false;
        }

        private void ProcessTiles(List<IMapTile> tiles, bool enable)
        {
            foreach (var tile in tiles)
            {
                if (!isMove)
                    tile.SetEnemy(enable);
                else
                {
                    if (!tile.HasObstacle && !tile.HasEnemy)
                        tile.SetWalkable(enable);
                }
            }
        }
    }
}