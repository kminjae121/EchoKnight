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

            Debug.Log(enemyPos);
            
            foreach (var tile in _tilesInRange)
            {
                Debug.Log(_targetEnemy);
                Debug.Log(tile.GridPos);
                
                if (tile.GridPos == enemyPos)
                {
                    Debug.Log(_targetEnemy);
                    _targetEnemy = enemy;
                    return;
                }
            }

            _targetEnemy = null;
        }
    }
}