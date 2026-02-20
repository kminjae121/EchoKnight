using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct SetAtkUIEvent : IEvent
    {
        public bool isLock;
        
        public SetAtkUIEvent(bool isLock)
        {
            this.isLock = isLock;
        }
    }
}