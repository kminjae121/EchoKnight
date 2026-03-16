using System;
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

        public event Action<MapTile> OnTileStateChanged; 
        
        public Vector2Int GridPos => gridPos;
        public Vector3 WorldPos => transform.position;
        
        public bool IsWalkable => isWalkable;
        public bool HasEnemy => hasEnemy;
        public bool HasObstacle => hasObstacle;
        public bool CanUnitPass => isWalkable && !hasObstacle;

        public void Initialize(Vector2Int pos)
        {
            gridPos = pos;
        }

        public void SetWalkable(bool walkable)
        {
            if (isWalkable == walkable)
                return;
            
            isWalkable = walkable;
            OnTileStateChanged?.Invoke(this);
        }

        public void SetEnemy(bool enemy)
        {
            if (hasEnemy == enemy)
                return;
            
            hasEnemy = enemy;
            OnTileStateChanged?.Invoke(this);
        }

        public void SetObstacle(bool obstacle)
        {
            if (hasObstacle == obstacle)
                return;
            
            hasObstacle = obstacle;
            OnTileStateChanged?.Invoke(this);
        }
    }
}