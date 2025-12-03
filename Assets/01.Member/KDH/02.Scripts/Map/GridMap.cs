using Code.Core.Interfaces;
using UnityEngine;

namespace Code.Map
{
    public class GridMap : MonoBehaviour, IGridMap
    {
        [Header("Map Settings")]
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float tileSize = 1f;
        
        [Header("Tile Prefab")]
        [SerializeField] private GameObject tilePrefab;
    
        private MapTile[,] tiles;
    
        public int Width => width;
        public int Height => height;
        public float TileSize => tileSize;
    
        private void Awake()
        {
            GenerateMap();
        }
    
        public void GenerateMap()
        {
            ClearMap();
            tiles = new MapTile[width, height];
    
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CreateTile(x, y);
                }
            }
        }
    
        private void CreateTile(int x, int y)
        {
            Vector3 worldPosition = GridToWorldPosition(x, y);
            
            GameObject tileObject;
            if (tilePrefab != null)
            {
                tileObject = Instantiate(tilePrefab, worldPosition, Quaternion.identity, transform);
            }
            else
            {
                tileObject = new GameObject($"Tile_{x}_{y}");
                tileObject.transform.position = worldPosition;
                tileObject.transform.parent = transform;
            }
    
            MapTile tile = tileObject.GetComponent<MapTile>();
            if (tile == null)
            {
                tile = tileObject.AddComponent<MapTile>();
            }
            
            tile. Initialize(new Vector2Int(x, y));
            tiles[x, y] = tile;
        }
    
        private void ClearMap()
        {
            if (tiles == null) return;
    
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles. GetLength(1); y++)
                {
                    if (tiles[x, y] != null)
                    {
                        DestroyImmediate(tiles[x, y].gameObject);
                    }
                }
            }
            tiles = null;
        }
    
        public Vector3 GridToWorldPosition(int x, int y)
        {
            return new Vector3(x * tileSize, 0f, y * tileSize) + transform.position;
        }
    
        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            Vector3 localPos = worldPosition - transform.position;
            int x = Mathf.RoundToInt(localPos.x / tileSize);
            int y = Mathf.RoundToInt(localPos. z / tileSize);
            return new Vector2Int(x, y);
        }
    
        public IMapTile GetTile(Vector2Int position)
        {
            return GetTile(position. x, position.y);
        }
    
        public IMapTile GetTile(int x, int y)
        {
            if (! IsValidPosition(new Vector2Int(x, y))) return null;
            return tiles[x, y];
        }
    
        public bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < width &&
                   position.y >= 0 && position.y < height;
        }
    
        public bool CanMoveTo(Vector2Int position)
        {
            IMapTile tile = GetTile(position);
            if (tile == null) return false;
            return tile.CanUnitPass;
        }
    }
}