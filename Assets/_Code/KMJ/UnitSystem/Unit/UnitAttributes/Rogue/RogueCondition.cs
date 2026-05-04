using System.Collections.Generic;
using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class RogueCondition : MonoBehaviour, IUnitCondition
    {
        [SerializeField] private int startValue = 1;
        [SerializeField] private int endValue = 4;

        private readonly Dictionary<GameObject, int> targetCounts = new();

        public void Initialize(Unit unit)
        {
        }

        public bool CheckCondition(GameObject target)
        {
            if (target == null) return false;

            if (!targetCounts.TryGetValue(target, out int value))
                value = startValue;
            else
                value += 1;

            if (value >= endValue)
            {
                targetCounts.Remove(target);
                return true;
            }

            targetCounts[target] = value;
            return false;
        }
    }
}