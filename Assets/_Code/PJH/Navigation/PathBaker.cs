using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Code.Navigation
{
    public class PathBaker : MonoBehaviour
    {
        [SerializeField] private Tilemap groundMap;
        [SerializeField] private Tilemap obstacleMap;
        [SerializeField] private BakedDataSO bakedData;

        [SerializeField] private bool isDrawGizmo = true;
        [SerializeField] private bool isCornerCheck = true;
        [SerializeField] private Color nodeColor, edgeColor;
        
        [ContextMenu("Bake map data")]
        private void BakeMapData()
        {
            Debug.Assert(groundMap != null && obstacleMap != null, "Target tilemaps are null or empty");
            WritePointData();
            RecordNeighbors();
            WriteIfInUnityEditor();
        }

        private void WritePointData()
        {
            bakedData.ClearPoints();
            groundMap.CompressBounds();
            
            BoundsInt mapBound = groundMap.cellBounds;

            for (int x = mapBound.xMin; x < mapBound.xMax; x++)
            {
                for (int y = mapBound.yMin; y < mapBound.yMax; y++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y);

                    if (CanMovePosition(cellPosition)) //장애물이 없는 곳이면 포인트에 넣는다.
                    {
                        AddPoint(cellPosition);
                    }
                }
            }

            bakedData.Initialize();
        }

        private void AddPoint(Vector3Int cellPosition)
        {
            Vector3 worldPosition = groundMap.GetCellCenterWorld(cellPosition);
            bakedData.AddPoint(worldPosition, cellPosition);
        }


        private void RecordNeighbors()
        {
            foreach (NodeData nodeData in bakedData.points)
            {
                nodeData.neighbors.Clear();
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if(x == 0 && y ==0) continue;
                        
                        Vector3Int nextPoint = new Vector3Int(x, y) + nodeData.cellPos;
                        //해당 인접 노드가 존재하면
                        if (bakedData.GetNodeIfExist(nextPoint, out NodeData adjacentNode))
                        {
                            Debug.Log(nextPoint);
                            if (CheckCorner(nextPoint, nodeData.cellPos))
                            {
                                nodeData.AddNeighbor(adjacentNode); //인접노드를 추가한다.
                                //Debug.Log($"{adjacentNode.cellPos} is added");
                            }
                        }
                    }
                }
            }
        }

        private bool CheckCorner(Vector3Int nextPoint, Vector3Int currentPoint)
        {
            if (isCornerCheck == false) return true;

            //x축과, Y축에 장애물이 있는지 검사해서 있다면 false
            return CanMovePosition(new Vector3Int(nextPoint.x, currentPoint.y)) &&
                   CanMovePosition(new Vector3Int(currentPoint.x, nextPoint.y));
        }

        private bool CanMovePosition(Vector3Int cellPosition)
        {
            bool hasObstacle = obstacleMap.HasTile(cellPosition);
            bool hasGround = groundMap.HasTile(cellPosition);
            bool hasUpGround = groundMap.HasTile(cellPosition + Vector3Int.up);
            bool hasDownObstacle = obstacleMap.HasTile(cellPosition + Vector3Int.down);
            bool hasUpObstacle = obstacleMap.HasTile(cellPosition + Vector3Int.up);
            
            return (hasObstacle == false && hasGround) ||
                   (hasObstacle && hasGround && hasUpGround && hasDownObstacle && hasUpObstacle == false);
        }
        
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
            if (isDrawGizmo == false) return;

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
            Vector3 direction = end - start;

            Vector3 arrowStart = end - direction.normalized * 0.25f;
            Vector3 arrowEnd = end - direction.normalized * 0.15f;
            const float arrowSize = 0.05f;
            Vector3 normalDir = direction.normalized;
            Vector3 trianglePointA = arrowStart + (Quaternion.Euler(0, 0, -90) * normalDir) * arrowSize;
            Vector3 trianglePointB = arrowStart + Quaternion.Euler(0, 0, 90) * normalDir * arrowSize;
    
            Gizmos.DrawLine(start, arrowStart);
    
            Gizmos.DrawLine(trianglePointA, arrowEnd);
            Gizmos.DrawLine(trianglePointB, arrowEnd);
            Gizmos.DrawLine(trianglePointA, trianglePointB);
        }

#endif
        
    }
}