using Code.Core.Interfaces;
using Code.Map;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Code.Navigation
{
    public class PathBaker : MonoBehaviour
    {
        [SerializeField] private GridMap gridMap;
        [SerializeField] private BakedDataSO bakedData;

        [SerializeField] private bool isDrawGizmo = true;
        [SerializeField] private bool isCornerCheck = true;
        [SerializeField] private Color nodeColor, edgeColor;

        [ContextMenu("Bake map data")]
        private void BakeMapData()
        {
            gridMap ??= FindFirstObjectByType<GridMap>();

            Debug.Assert(gridMap != null, "GridMap is null");
            Debug.Assert(bakedData != null, "BakedDataSO is null");

            if (gridMap == null || bakedData == null)
                return;

            WritePointData();
            RecordNeighbors();
            WriteIfInUnityEditor();
        }

        private void WritePointData()
        {
            bakedData.ClearPoints();

            for (int x = 0; x < gridMap.Width; ++x)
            for (int y = 0; y < gridMap.Height; ++y)
            {
                var gridPos = new Vector2Int(x, y);

                if (CanMovePosition(gridPos))
                    AddPoint(gridPos);
            }

            bakedData.Initialize();
        }

        private void AddPoint(Vector2Int gridPos)
        {
            IMapTile tile = gridMap.GetTile(gridPos);

            if (tile == null)
                return;

            bakedData.AddPoint(tile.WorldPos, GridToCell(gridPos));
        }

        private void RecordNeighbors()
        {
            Vector2Int[] directions =
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                new(1, 1), new(1, -1),
                new(-1, 1), new(-1, -1)
            };

            foreach (NodeData nodeData in bakedData.points)
            {
                nodeData.neighbors.Clear();

                Vector2Int currentPoint = CellToGrid(nodeData.cellPos);

                foreach (var dir in directions)
                {
                    Vector2Int nextPoint = currentPoint + dir;

                    if (!bakedData.GetNodeIfExist(GridToCell(nextPoint), out NodeData adjacentNode))
                        continue;

                    if (CheckCorner(nextPoint, currentPoint))
                        nodeData.AddNeighbor(adjacentNode);
                }
            }
        }

        private bool CheckCorner(Vector2Int nextPoint, Vector2Int currentPoint)
        {
            if (!isCornerCheck)
                return true;

            Vector2Int direction = nextPoint - currentPoint;

            if (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) <= 1)
                return true;

            return CanMovePosition(new Vector2Int(nextPoint.x, currentPoint.y)) &&
                   CanMovePosition(new Vector2Int(currentPoint.x, nextPoint.y));
        }

        private bool CanMovePosition(Vector2Int gridPos)
        {
            if (gridMap == null || !gridMap.IsValidPosition(gridPos))
                return false;

            IMapTile tile = gridMap.GetTile(gridPos);

            return tile != null &&
                   tile.HasState(TileState.Walkable) &&
                   !tile.HasState(TileState.Obstacle);
        }

        private static Vector3Int GridToCell(Vector2Int gridPos)
            => new(gridPos.x, gridPos.y, 0);

        private static Vector2Int CellToGrid(Vector3Int cellPos)
            => new(cellPos.x, cellPos.y);

        private void WriteIfInUnityEditor()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(bakedData);
            AssetDatabase.SaveAssets();
#endif
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!isDrawGizmo || bakedData == null)
                return;

            foreach (NodeData nodeData in bakedData.points)
            {
                Gizmos.color = nodeColor;
                Gizmos.DrawWireSphere(nodeData.worldPos, 0.15f);

                foreach (LinkData link in nodeData.neighbors)
                {
                    Gizmos.color = edgeColor;
                    DrawLineGizmo(link.startPos, link.endPos);
                }
            }
        }

        private void DrawLineGizmo(Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;

            if (dir.sqrMagnitude <= Mathf.Epsilon)
                return;

            Vector3 arrowStart = end - dir.normalized * 0.25f;
            Vector3 arrowEnd = end - dir.normalized * 0.15f;
            Vector3 right = Vector3.Cross(Vector3.up, dir.normalized);
            const float arrowSize = 0.05f;
            Vector3 trianglePointA = arrowStart + right * arrowSize;
            Vector3 trianglePointB = arrowStart - right * arrowSize;

            Gizmos.DrawLine(start, arrowStart);
            Gizmos.DrawLine(trianglePointA, arrowEnd);
            Gizmos.DrawLine(trianglePointB, arrowEnd);
            Gizmos.DrawLine(trianglePointA, trianglePointB);
        }
#endif
    }
}