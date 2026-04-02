using System;
using System.Threading.Tasks;
using Code.Core.Debugs;
using Code.Core.Interfaces;
using Code.Map;
using Code.Navigation;
using UnityEngine;

namespace Code.UnitSystem.UnitComponent
{
    public class PathMover : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private int maxPathCount = 50;
        [SerializeField] private int movePoint = 8;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private Vector3[] pointArray;

        public event Action OnMoveEnd;

        private PathAgent _pathAgent;
        private Unit _owner;
        private GridMap _gridMap;
        private UnitRotation _rotationCompo;
        private int _pathLength;

        public void Initialize(Unit owner)
        {
            _owner = owner;
            _pathAgent = owner.GetComponent<PathAgent>();
            _gridMap = GridMap.Instance;
            _rotationCompo = owner.GetUnitCompo<UnitRotation>();
            pointArray = new Vector3[maxPathCount];
        }

        public void SetPathAndMove(Vector2Int startPos, Vector2Int destination)
        {
            SetPathAndMove(GridToCell(startPos), GridToCell(destination));
        }

        public async void SetPathAndMove(Vector3Int startPos, Vector3Int destination)
        {
            try
            {
                if (_pathAgent == null)
                {
                    UnityLogger.LogError("PathAgent is missing.");
                    OnMoveEnd?.Invoke();
                    return;
                }

                _gridMap ??= GridMap.Instance;

                if (_gridMap == null)
                {
                    UnityLogger.LogError("GridMap is missing.");
                    OnMoveEnd?.Invoke();
                    return;
                }

                _rotationCompo ??= _owner.GetUnitCompo<UnitRotation>();

                UnityLogger.Log($"Start : {startPos}, Destination : {destination}");
                _pathLength = await _pathAgent.GetPath(startPos, destination, pointArray);

                if (_pathLength <= 0)
                {
                    UnityLogger.Log("pathLength is zero");
                    OnMoveEnd?.Invoke();
                    return;
                }

                int remainingMovePoint = movePoint;
                Vector3Int previousCell = startPos;
                Vector3Int finalCell = startPos;

                for (int i = 1; i < _pathLength; ++i)
                {
                    Vector3Int nextCell = GridToCell(_gridMap.WorldToGridPosition(pointArray[i]));
                    Vector3Int reachableCell =
                        GetReachableCell(previousCell, nextCell, remainingMovePoint, out int movedCost);

                    if (movedCost <= 0)
                        break;

                    Vector3 targetPoint = GetWorldPosition(reachableCell);
                    RotateToPoint(targetPoint);
                    await MoveToPoint(targetPoint);

                    remainingMovePoint -= movedCost;
                    previousCell = reachableCell;
                    finalCell = reachableCell;

                    if (reachableCell != nextCell || remainingMovePoint <= 0)
                        break;
                }

                UpdateUnitTileState(startPos, finalCell);
                OnMoveEnd?.Invoke();
            }
            catch (Exception e)
            {
                UnityLogger.LogError(e.Message);
                OnMoveEnd?.Invoke();
            }
        }

        private async Task MoveToPoint(Vector3 point)
        {
            while (Vector3.Distance(_owner.transform.position, point) > 0.1f)
            {
                _owner.transform.position =
                    Vector3.MoveTowards(_owner.transform.position, point, moveSpeed * Time.deltaTime);
                await Awaitable.NextFrameAsync();
            }
        }

        private void RotateToPoint(Vector3 point)
        {
            if (_rotationCompo == null)
                return;

            _rotationCompo.SetDir(point);
        }

        private Vector3Int GetReachableCell(Vector3Int startCell, Vector3Int endCell, int remainingMovePoint, out int movedCost)
        {
            movedCost = 0;
            Vector3Int delta = endCell - startCell;
            int segmentCost = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));

            if (segmentCost <= 0 || remainingMovePoint <= 0)
                return startCell;

            Vector3Int direction = new Vector3Int
            (
                Math.Sign(delta.x),
                Math.Sign(delta.y),
                Math.Sign(delta.z)
            );

            Vector3Int currentCell = startCell;
            int maxStep = Mathf.Min(segmentCost, remainingMovePoint);

            for (int step = 0; step < maxStep; ++step)
            {
                Vector3Int nextCell = currentCell + direction;

                if (!CanTraverseCell(nextCell))
                    break;

                currentCell = nextCell;
                movedCost++;
            }

            return currentCell;
        }

        private Vector3 GetWorldPosition(Vector3Int cellPosition)
        {
            Vector3 worldPosition = _gridMap.GridToWorldPosition(cellPosition.x, cellPosition.y);
            return ToMovePoint(worldPosition);
        }

        private Vector3 ToMovePoint(Vector3 worldPosition)
            => new(worldPosition.x, _owner.transform.position.y, worldPosition.z);

        private static Vector3Int GridToCell(Vector2Int gridPosition)
            => new(gridPosition.x, gridPosition.y, 0);

        private bool CanTraverseCell(Vector3Int cellPosition)
        {
            if (_gridMap == null)
                return false;

            return _gridMap.CanMoveTo(new Vector2Int(cellPosition.x, cellPosition.y));
        }

        private void UpdateUnitTileState(Vector3Int previousCell, Vector3Int currentCell)
        {
            if (_owner == null || _gridMap == null)
                return;

            IMapTile previousTile = _gridMap.GetTile(previousCell.x, previousCell.y);
            IMapTile currentTile = _gridMap.GetTile(currentCell.x, currentCell.y);

            if (previousTile != null)
            {
                previousTile.SetState(TileState.Enemy, false);
                previousTile.SetState(TileState.Obstacle, false);
                previousTile.SetState(TileState.Walkable, true);
            }

            if (currentTile != null)
            {
                currentTile.SetState(TileState.Walkable, false);
                currentTile.SetState(TileState.Obstacle, true);

                if (_owner.IsPlayerUnit)
                    currentTile.SetState(TileState.Enemy, false);
                else
                    currentTile.SetState(TileState.Enemy, true);
            }
        }
    }
}
