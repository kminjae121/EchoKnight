using Code.UnitSystem.GimicSystem;
using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct UnitGimicEvent : IEvent
    {
        public GameObject target;
        public UnitType unitType;
        public GimicOption gimicOption;
        
        public UnitGimicEvent(UnitType unitType, GameObject target, GimicOption gimiOption)
        {
            this.unitType = unitType;
            this.target = target;
            this.gimicOption = gimiOption;
        }
    }
}