using System;
using Code.Core.Debugs;
using Code.Map;
using Code.UnitSystem.UnitComponent;
using Code.Utils;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.UnitSystem.Enemies.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "MoveToTarget", story: "[Enemy] move to [Target]", category: "Action", id: "b573e234c5921f41ffd38ca38e3e3074")]
    public partial class MoveToTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemyUnit> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private static readonly Vector2Int[] Offsets =
        {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        private PathMover _mover;
        private GridMap _gridMap;
        private bool _isMoving;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Target.Value == null)
                return Status.Failure;
            
            _gridMap = GridMap.Instance;
            _mover = Enemy.Value.PathMover;

            if (_gridMap == null)
            {
                UnityLogger.LogError("GridMap is missing.");
                return Status.Failure;
            }

            if (_mover == null)
            {
                UnityLogger.LogError("PathMover is missing.");
                return Status.Failure;
            }

            Vector2Int startPos = _gridMap.WorldToGridPosition(Enemy.Value.transform.position);
            Vector2Int targetPos = _gridMap.WorldToGridPosition(Target.Value.transform.position);

            if (!TryGetMoveDestination(startPos, targetPos, out Vector2Int destination))
                return Status.Success;

            if (destination == startPos)
                return Status.Success;

            _isMoving = true;
            _mover.OnMoveEnd += HandleMovementEnd;
            _mover.SetPathAndMove(startPos, destination, true);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _isMoving ? Status.Running : Status.Success;
        }

        protected override void OnEnd()
        {
            if (_mover != null)
                _mover.OnMoveEnd -= HandleMovementEnd;
        }

        private void HandleMovementEnd()
        {
            _isMoving = false;
        }

        private bool TryGetNearestTile(Vector2Int sourceTile, Vector2Int targetTile, out Vector2Int nearTile)
        {
            float minDistance = Mathf.Infinity;
            nearTile = default;
            bool found = false;

            if (DistanceUtils.GetEuclideanDistance(sourceTile, targetTile) <= 1f)
            {
                nearTile = sourceTile;
                return true;
            }

            foreach (Vector2Int offset in Offsets)
            {
                Vector2Int nextTile = targetTile + offset;

                if (!_gridMap.IsValidPosition(nextTile))
                    continue;

                if (!_gridMap.CanMoveTo(nextTile))
                    continue;

                float distance = DistanceUtils.GetEuclideanDistance(sourceTile, nextTile);

                if (distance >= minDistance)
                    continue;

                UnityLogger.Log($"distance set : {distance}");
                minDistance = distance;
                nearTile = nextTile;
                found = true;
            }

            return found;
        }

        private bool TryGetMoveDestination(Vector2Int sourceTile, Vector2Int targetTile, out Vector2Int destination)
        {
            if (TryGetNearestTile(sourceTile, targetTile, out destination))
                return true;

            destination = targetTile;
            return true;
        }
    }
}
