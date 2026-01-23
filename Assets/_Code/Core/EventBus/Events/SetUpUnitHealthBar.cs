using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct SetUpUnitHealthBar : IEvent
    {
        public int unitCount;

        public float currentValue;
        
        public float maxValue;

        public float finalValue;
        
        public SetUpUnitHealthBar(int unitCount, float currentValue, float maxValue)
        {
            this.unitCount = unitCount;
            this.currentValue = currentValue;
            this.maxValue = maxValue;
            finalValue = Mathf.Clamp01(currentValue / maxValue);
        }
    }
}