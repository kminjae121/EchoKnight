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
        public bool CanUnitPass => isWalkable && !hasObstacle;

        private MapTileVisual _visual;

        private void Awake()
        {
            _visual = GetComponentInChildren<MapTileVisual>();
        }

        public void Initialize(Vector2Int pos)
        {
            gridPos = pos;
        }

        public void SetWalkable(bool walkable)
        {
            if (isWalkable == walkable)
                return;
            
            isWalkable = walkable;
            _visual.HandleTileChanged(this);
        }

        public void SetEnemy(bool enemy)
        {
            if (hasEnemy == enemy)
                return;
            
            hasEnemy = enemy;
            _visual.HandleTileChanged(this);
        }

        public void SetObstacle(bool obstacle)
        {
            if (hasObstacle == obstacle)
                return;
            
            hasObstacle = obstacle;
            _visual.HandleTileChanged(this);
        }
        
        public void SetDecalActive(bool isActive)
        {
            _visual?.SetDecalActive(isActive);
        }
    }
}