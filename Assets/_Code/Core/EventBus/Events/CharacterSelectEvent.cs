namespace Code.Core.Events.Bus
{
    public struct CharacterSelectEvent : IEvent
    {
        public UnitSO Unit { get; }

        public CharacterSelectEvent(UnitSO unit)
        {
            Unit = unit;
        }
    }
}