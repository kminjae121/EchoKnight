using System;
using System.Threading.Tasks;
using Code.Core.Debugs;
using Code.Map;
using Code.Navigation;
using UnityEngine;

namespace Code.UnitSystem.UnitComponent
{
    public class PathMover : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private int maxPathCount = 50; // 최대 점 개수
        [SerializeField] private int movePoint = 8;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private Vector3[] pointArray;
        
        public event Action OnMoveEnd;

        private PathAgent _pathAgent;
        private Unit _owner;
        private GridMap _gridMap;
        private int _pathLength;

        public void Initialize(Unit owner)
        {
            _owner = owner;
            _pathAgent = owner.GetComponent<PathAgent>();
            _gridMap = GridMap.Instance;
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

                for (int i = 1; i < _pathLength; ++i)
                {
                    Vector3Int nextCell = GridToCell(_gridMap.WorldToGridPosition(pointArray[i]));
                    int segmentCost = GetSegmentCost(previousCell, nextCell);

                    if (segmentCost <= 0)
                    {
                        previousCell = nextCell;
                        continue;
                    }

                    if (remainingMovePoint >= segmentCost)
                    {
                        await MoveToPoint(ToMovePoint(pointArray[i]));
                        remainingMovePoint -= segmentCost;
                        previousCell = nextCell;

                        if (remainingMovePoint <= 0)
                            break;

                        continue;
                    }

                    if (remainingMovePoint > 0)
                    {
                        Vector3Int partialCell = GetReachableCell(previousCell, nextCell, remainingMovePoint);
                        await MoveToPoint(GetWorldPosition(partialCell));
                    }

                    break;
                }

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

        private int GetSegmentCost(Vector3Int startCell, Vector3Int endCell)
        {
            Vector3Int delta = endCell - startCell;
            return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
        }

        private Vector3Int GetReachableCell(Vector3Int startCell, Vector3Int endCell, int remainingMovePoint)
        {
            Vector3Int delta = endCell - startCell;
            Vector3Int direction = new Vector3Int
            (
                Math.Sign(delta.x),
                Math.Sign(delta.y),
                Math.Sign(delta.z)
            );

            return startCell + direction * remainingMovePoint;
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
    }
}