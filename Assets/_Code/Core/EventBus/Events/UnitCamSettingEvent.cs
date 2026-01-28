using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct UnitCamSettingEvent : IEvent
    {
        public GameObject target;
        public bool isLocking;

        public UnitCamSettingEvent(GameObject target , bool isLocking)
        {
            this.target = target;
            this.isLocking = isLocking;
        }
    }
}