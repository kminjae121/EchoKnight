using Code.Core.Interfaces;
using UnityEngine;

namespace Code.Map
{
    public class MapTile : MonoBehaviour, IMapTile
    {
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private bool isWalkable = true;
        [SerializeField] private bool hasEnemy = false;
        [SerializeField] private bool hasObstacle = false;

        public Vector2Int GridPosition => gridPosition;
        public bool IsWalkable => isWalkable;
        public bool HasEnemy => hasEnemy;
        public bool HasObstacle => hasObstacle;
    
        public bool CanUnitPass => isWalkable && ! hasObstacle;

        public void Initialize(Vector2Int position)
        {
            gridPosition = position;
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
    }
}