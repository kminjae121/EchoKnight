using UnityEngine;

namespace Code.UnitSystem.Combat
{
    public class CriticalSpot : MonoBehaviour, IUnitComponent
    {
        private CharacterUnit _characterUnit;
        
        public void Initialize(Unit owner)
        {
            _characterUnit = owner as CharacterUnit;
        }
        
        public float CheckEnemyBody(DamageData damageData, GameObject target, float atkDamage)
        {
            atkDamage = damageData.damage;
            
            Vector3 toAttacker = _characterUnit.transform.position - target.transform.position;
            toAttacker.y = 0f;

            Vector3 enemyForward = target.transform.forward;
            enemyForward.y = 0f;

            toAttacker.Normalize();
            enemyForward.Normalize();

            float dot = Vector3.Dot(enemyForward, toAttacker);
            
            float deadZone = 0.2f;

            BodyType type =
                dot > deadZone ? BodyType.Head :
                dot < -deadZone ? BodyType.Back :
                BodyType.None;

            if (_characterUnit.unitSO.EntityType == EntityType.MeleeAttacker && type == BodyType.Head)
            {
                return atkDamage * 1.2f;
            }
            else if (_characterUnit.unitSO.EntityType == EntityType.LongRanger && type == BodyType.Back)
            {
                return atkDamage * 1.2f;
            }
            else
                return 0;
        }
    }
}