using Code.UnitSystem;
using UnityEngine;

namespace Code.UnitSystem.Combat
{
    public interface IDamageable
    {
        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, AttackDataSO attackData, Unit dealer, bool isCritical);
    }
}