namespace Code.Core.Events.Bus
{
    public struct CharacterDeselectEvent : IEvent
    {
        public UnitSO Unit { get; }

        public CharacterDeselectEvent(UnitSO unit)
        {
            Unit = unit;
        }
    }
}