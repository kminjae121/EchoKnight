using Code.Core.Interfaces;
using UnityEngine;

namespace Code.Map
{
    public class MapTile : MonoBehaviour, IMapTile
    {
        [SerializeField] private Vector2Int gridPos;
        [SerializeField] private bool isWalkable = true;
        [SerializeField] private bool hasEnemy;
        [SerializeField] private bool hasObstacle;

        public Vector2Int GridPos => gridPos;
        public Vector3 WorldPos => transform.position;
        
        public bool IsWalkable => isWalkable;
        public bool HasEnemy => hasEnemy;
        public bool HasObstacle => hasObstacle;
        public bool CanUnitPass => isWalkable && ! hasObstacle;

        public void Initialize(Vector2Int pos)
        {
            gridPos = pos;
        }

        public void SetWalkable(bool walkable)
        {
            isWalkable = walkable;
        }

        public void SetEnemy(bool enemy)
        {
            hasEnemy = enemy;
        }

        public void SetObstacle(bool obstacle)
        {
            hasObstacle = obstacle;
        }

        public void SetDecalActive(bool isActive)
        {
            var visual = GetComponentInChildren<MapTileVisual>();
            
            if (visual != null)
                visual.SetDecalActive(isActive);
        }
    }
}