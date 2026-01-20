using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct ApSliderEvent : IEvent
    {
        public float value;

        public ApSliderEvent(float value)
        {
            this.value = value;
        }
    }
}