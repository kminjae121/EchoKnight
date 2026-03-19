using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct DamageEvent : IEvent
    {
        public DamageData DamageData;

        public AttackDataSO atkData;

        public GameObject target;

        public DamageEvent(DamageData data, AttackDataSO atkData, GameObject target)
        {
            DamageData = data;
            this.atkData = atkData;
            this.target = target;
        }
    }
}