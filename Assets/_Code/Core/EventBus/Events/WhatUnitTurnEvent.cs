namespace Code.Core.Events.Bus
{
    public struct WhatUnitTurnEvent : IEvent
    {
        public UnitType unitType;
        
        public WhatUnitTurnEvent(UnitType unitType)
        {
            this.unitType = unitType;
        }
    }
}