using Code.UnitSystem;
using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct DamageEvent : IEvent
    {
        public DamageData DamageData;

        public AttackDataSO atkData;

        public GameObject target;
        public float addDamage;

        public Unit Owner;

        public bool IsCritical;

        public bool IsPenetrate;
        public DamageEvent(DamageData data, AttackDataSO atkData, GameObject target, float addDamage, Unit Owenr,  bool IsCritical, bool IsPenetrate)
        {
            DamageData = data;
            this.atkData = atkData;
            this.target = target;
            this.addDamage = addDamage;
            this.Owner = Owenr;
            this.IsCritical = IsCritical;
            this.IsPenetrate = IsPenetrate;
        }
    }
}