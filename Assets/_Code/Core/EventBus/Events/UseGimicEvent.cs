namespace Code.Core.Events.Bus
{
    public struct UseGimicEvent : IEvent
    {
        public UnitType unitType;

        public UseGimicEvent(UnitType unitType)
        {
            this.unitType = unitType;
        }
    }
}