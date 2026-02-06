using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct SetAtkUIEvent : IEvent
    {
        public GameObject Object;
        
        public SetAtkUIEvent(GameObject obj)
        {
            Object = obj;
        }
    }
}