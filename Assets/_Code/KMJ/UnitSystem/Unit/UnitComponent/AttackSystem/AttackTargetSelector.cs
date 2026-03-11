using System.Runtime.CompilerServices;
using Code.Core.Debugs;
using Code.Map;
using Code.UnitSystem;
using EnemySystem;
using UnityEngine;

namespace Code.AttackSystem
{
    public class AttackTargetSelector : RangeComponent
    {
        public GameObject _targetEnemy { get; private set; }
        public EnemyTargeting _targetingCompo { get; private set; }
        
        
        public void SetTargeting(EnemyTargeting targetingCompo) => 
            _targetingCompo = targetingCompo;
        
        public void FindEnemyIsThere(GameObject enemy)
        {
            if (enemy == null)
            {
                _targetEnemy = null;
                return;
            }

            Debug.Log(enemy);
            
            if (_targetEnemy != null && _targetEnemy != enemy)
                _targetingCompo?.OffTargeting();
            
            Vector2Int enemyPos = GridMap.Instance.WorldToGridPosition(enemy.transform.position);
            
            foreach (var tile in _tilesInRange)
            {
                Debug.Log($"타일은 {tile.GridPos}");
                Debug.Log($"적 위치는 {enemyPos}");
                
                if (tile.GridPos == enemyPos)
                {
                    Debug.Log("드디어 됐다.");
                    _targetEnemy = enemy;
                    return;
                }
            }

            _targetEnemy = null;
        }
    }
}