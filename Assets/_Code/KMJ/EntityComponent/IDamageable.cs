using Code.UnitSystem;
using UnitSystem;
using UnityEngine;

namespace Code.EntityComponent
{
    public interface IDamageable
    {
        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, AttackDataSO attackData, Unit dealer);
    }
}