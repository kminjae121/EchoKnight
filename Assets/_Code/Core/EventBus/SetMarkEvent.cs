using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct SetMarkEvent : IEvent
    {
        public GameObject target;

        public SetMarkEvent(GameObject target)
        {
            this.target = target;
        }
    }
}