using UnityEngine;

namespace Code.Core.Interfaces
{
    public interface IMapTile
    {
        Vector2Int GridPos { get; }
        bool IsWalkable { get;  } 
        bool HasEnemy { get; }
        bool HasObstacle { get;  }
        bool CanUnitPass { get;  }
    
        void SetWalkable(bool walkable);
        void SetEnemy(bool hasEnemy);
        void SetObstacle(bool hasObstacle);
    }
}

