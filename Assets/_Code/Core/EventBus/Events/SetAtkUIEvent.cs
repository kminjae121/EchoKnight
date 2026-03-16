namespace Code.Core.Events.Bus
{
    public struct SetAtkUIEvent : IEvent
    {
        public bool IsLock { get; private set; }
        
        public SetAtkUIEvent(bool isLock)
        {
            IsLock = isLock;
        }
    }
}