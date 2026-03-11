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

            if (_targetEnemy != null && _targetEnemy != enemy) _targetingCompo?.OffTargeting();

            _targetEnemy = null;

            foreach (var obj in _verticalCollider)
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                    return;
                }

            foreach (var obj in _horizontalCollider)
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                    return;
                }
        }
    }
}