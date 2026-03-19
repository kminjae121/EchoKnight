using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct UnitGimicEvent : IEvent
    {
        public UnitType unitType;
        public GameObject target;
        
        public UnitGimicEvent(UnitType unitType, GameObject target = null)
        {
            this.unitType = unitType;
            this.target = target;
        }
    }
}