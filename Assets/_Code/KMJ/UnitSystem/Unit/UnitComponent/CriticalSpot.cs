using Code.EntityComponent;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;
namespace Code.UnitSystem
{
    public class CriticalSpot : MonoBehaviour, IUnitComponent
    {
        private CharacterUnit _characterUnit;
        
        public void Initialize(Code.UnitSystem.Unit owner)
        {
            _characterUnit = owner as CharacterUnit;
        }
        
        public void CheckEnemyBody(DamageData damageData,GameObject target, float atkDamage, float addDamage)
        {
            damageData.damage = atkDamage;
            addDamage = 0;
            
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
                addDamage = damageData.damage * 0.4f;
            }
            else if (_characterUnit.unitSO.EntityType == EntityType.LongRanger && type == BodyType.Back)
            {
                addDamage = damageData.damage * 0.4f;
            }
            else
            {
                addDamage = 0f;
            }
        }

    }
}