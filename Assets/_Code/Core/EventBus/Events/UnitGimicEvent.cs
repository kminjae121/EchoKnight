namespace Code.Core.Events.Bus
{
    public struct UnitGimicEvent : IEvent
    {
        public UnitType unitType;
        
        public UnitGimicEvent(UnitType unitType)
        {
            this.unitType = unitType;
        }
    }
}