using Code.Core.Interfaces;
using UnityEngine;

namespace Code.Map
{
    [RequireComponent(typeof(GridMap))]
    public class GridMapGizmos : MonoBehaviour
    {
        [Header("Gizmo Settings")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private float gizmoHeight = 0.5f;

        [Header("Colors")]
        [SerializeField] private Color walkableColor = Color.green;
        [SerializeField] private Color nonWalkableColor = Color.gray;
        [SerializeField] private Color enemyColor = Color.red;
        [SerializeField] private Color obstacleColor = Color.yellow;
        [SerializeField] private Color blockedColor = Color.magenta;
        [SerializeField] private Color gridLineColor = Color.white;
    
        private GridMap gridMap;
    
        private void OnValidate()
        {
            gridMap = GetComponent<GridMap>();
        }
    
        private void OnDrawGizmos()
        {
            if (! showGizmos) return;
            
            gridMap = GetComponent<GridMap>();
            if (gridMap == null) return;
    
            DrawGridLines();
            DrawTileStates();
        }
    
        private void DrawGridLines()
        {
            Gizmos.color = gridLineColor;
            float tileSize = gridMap.TileSize;
    
            for (int x = 0; x <= gridMap.Width; x++)
            {
                Vector3 start = transform.position + new Vector3(x * tileSize - tileSize * 0.5f, gizmoHeight, -tileSize * 0.5f);
                Vector3 end = transform.position + new Vector3(x * tileSize - tileSize * 0.5f, gizmoHeight, gridMap.Height * tileSize - tileSize * 0.5f);
                Gizmos.DrawLine(start, end);
            }
    
            for (int y = 0; y <= gridMap.Height; y++)
            {
                Vector3 start = transform. position + new Vector3(-tileSize * 0.5f, gizmoHeight, y * tileSize - tileSize * 0.5f);
                Vector3 end = transform.position + new Vector3(gridMap. Width * tileSize - tileSize * 0.5f, gizmoHeight, y * tileSize - tileSize * 0.5f);
                Gizmos.DrawLine(start, end);
            }
        }
    
        private void DrawTileStates()
        {
            float tileSize = gridMap.TileSize;
            Vector3 cubeSize = new Vector3(tileSize * 0.9f, 0.1f, tileSize * 0.9f);
    
            for (int x = 0; x < gridMap.Width; x++)
            {
                for (int y = 0; y < gridMap. Height; y++)
                {
                    IMapTile tile = gridMap.GetTile(x, y);
                    if (tile == null) continue;
    
                    Vector3 position = gridMap.GridToWorldPosition(x, y) + Vector3.up * gizmoHeight;
                    
                    Gizmos.color = GetTileColor(tile);
                    Gizmos.DrawCube(position, cubeSize);
                }
            }
        }
    
        private Color GetTileColor(IMapTile tile)
        {
            if (tile.HasEnemy)
                return enemyColor;
            
            if (tile. HasObstacle)
                return obstacleColor;
            
            if (! tile.IsWalkable)
                return nonWalkableColor;
            
            if (! tile. CanUnitPass)
                return blockedColor;
            
            return walkableColor;
        }
    }
}