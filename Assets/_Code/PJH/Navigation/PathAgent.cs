using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Code.Map;
using Code.Core.Debugs;
using Code.UnitSystem;
using Code.Utils;
using UnityEngine;

namespace Code.Navigation
{
    public class PathAgent : MonoBehaviour
    {
        [SerializeField] private BakedDataSO bakedData;

        private CancellationTokenSource _cts = new();
        private bool _isCalculating;
        
        public async Task<int> GetPath(Vector3Int startPos, Vector3Int destination, Vector3[] pointArr)
        {
            if (_isCalculating && _cts != null)
                _cts.Cancel();

            if (_cts is { IsCancellationRequested: true })
                _cts = new CancellationTokenSource();

            try
            {
                _isCalculating = true;
                HashSet<Vector3Int> blockedCells = CollectBlockedCells(startPos, destination);

                (List<AstarNode> list, bool isSuccess) =
                    await Task.Run(() => CalculatePath(startPos, destination, blockedCells), _cts.Token);

                _isCalculating = false;

                int cornerIndex = 0;

                if (!isSuccess)
                {
                    UnityLogger.Log("Calculation Failed");
                    return cornerIndex;
                }

                pointArr[cornerIndex] = list[0].worldPos;
                ++cornerIndex;

                for (int i = 1; i < list.Count - 1; ++i)
                {
                    if (cornerIndex >= pointArr.Length)
                        break;

                    pointArr[cornerIndex] = list[i].worldPos;
                    ++cornerIndex;
                }

                pointArr[cornerIndex] = list[^1].worldPos;
                ++cornerIndex;

                return cornerIndex;
            }
            catch (OperationCanceledException)
            {
                return -1;
            }
            catch (Exception ex)
            {
                UnityLogger.Log(ex.Message);
                return -1;
            }
            finally
            {
                _isCalculating = false;
            }
        }

        private (List<AstarNode>, bool) CalculatePath(Vector3Int startPoint, Vector3Int destination, HashSet<Vector3Int> blockedCells)
        {
            UnityLogger.Log("Calculate 진입");
            
            PriorityQueue<AstarNode> openList = new();
            HashSet<Vector3Int> closedSet = new();
            Dictionary<Vector3Int, float> bestGByCell = new();
            List<AstarNode> path = new();
            
            bool result = false;
            AstarNode goalNode = null;

            bool startSuccess = bakedData.GetNodeIfExist(startPoint, out var startNode);
            bool endSuccess = bakedData.GetNodeIfExist(destination, out var endNode);
            UnityLogger.Log($"st : {startPoint}, {startSuccess}, ed : {destination}, {endSuccess}");
            
            if (!startSuccess || !endSuccess)
                return (path, false);

            var startAstarNode = new AstarNode
            {
                nodeData = startNode,
                cellPos = startNode.cellPos,
                worldPos = startNode.worldPos,
                parentNode = null,
                g = 0,
                f = CalculateH(startNode.cellPos, endNode.cellPos)
            };

            openList.Push(startAstarNode);
            bestGByCell[startAstarNode.cellPos] = startAstarNode.g;
            
            while (openList.Count > 0)
            {
                if (_cts.Token.IsCancellationRequested)
                    throw new OperationCanceledException(_cts.Token);

                AstarNode currentNode = openList.Pop();

                if (closedSet.Contains(currentNode.cellPos))
                    continue;

                if (bestGByCell.TryGetValue(currentNode.cellPos, out float bestKnownG)
                    && currentNode.g > bestKnownG)
                    continue;

                closedSet.Add(currentNode.cellPos);

                if (currentNode.nodeData == endNode)
                {
                    result = true;
                    goalNode = currentNode;
                    break;
                }

                foreach (var link in currentNode.nodeData.neighbors)
                {
                    if (closedSet.Contains(link.endCellPos))
                        continue;

                    if (blockedCells != null && blockedCells.Contains(link.endCellPos))
                        continue;

                    if (!bakedData.GetNodeIfExist(link.endCellPos, out NodeData nextNode))
                        continue;

                    float newG = currentNode.g + link.cost;

                    if (bestGByCell.TryGetValue(nextNode.cellPos, out float oldG) && newG >= oldG)
                        continue;

                    bestGByCell[nextNode.cellPos] = newG;

                    openList.Push(new AstarNode
                    {
                        nodeData = nextNode,
                        cellPos = nextNode.cellPos,
                        worldPos = nextNode.worldPos,
                        parentNode = currentNode,
                        g = newG,
                        f = newG + CalculateH(nextNode.cellPos, endNode.cellPos)
                    });
                }
            }

            if (result)
            {
                AstarNode last = goalNode;

                while (last.parentNode != null)
                {
                    path.Add(last);
                    last = last.parentNode;
                }

                path.Add(last); // 시작점
                path.Reverse();
            }
            
            return (path, result);
        }

        private HashSet<Vector3Int> CollectBlockedCells(Vector3Int startPos, Vector3Int destination)
        {
            var blockedCells = new HashSet<Vector3Int>();
            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                return blockedCells;

            for (int y = 0; y < gridMap.Height; ++y)
            {
                for (int x = 0; x < gridMap.Width; ++x)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);

                    if (cellPos == startPos || cellPos == destination)
                        continue;

                    var tile = gridMap.GetTile(x, y);

                    if (tile == null)
                        continue;

                    if (tile.HasAnyState(TileState.Enemy | TileState.Obstacle))
                        blockedCells.Add(cellPos);
                }
            }

            return blockedCells;
        }

        private float CalculateH(Vector3Int startPoint, Vector3Int destination)
        {
            // 유클리드
            return Vector3Int.Distance(startPoint, destination);
            
            // 옥타일
            // int dx = Mathf.Abs(startPoint.x - destination.x);
            // int dy = Mathf.Abs(startPoint.y - destination.y);
            //
            // int min = Mathf.Min(dx, dy);
            // int max = Mathf.Max(dx, dy);
            //
            // return min * Mathf.Sqrt(2)+ (max - min);
        }
    }
}