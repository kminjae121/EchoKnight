using UnityEngine;

namespace EnemySystem
{
    [CreateAssetMenu(fileName = "EnemySO", menuName = "EnemySO/enemySO", order = 0)]
    public class EnemySO : ScriptableObject
    {
        [Header("EnemyName")]
        public string EnemyName;
        
        [Space(10)]
        [Header("Enemy Settings")]
        public bool isLongRange;

        public float turnSpeed;

        public bool isPlayerUnit = false;

        public float turnGauge;
    
        public float moveSpeed;
        
        [Header("enemyType")]
        public EntityType entityType = EntityType.MeleeAttacker;
    }
}