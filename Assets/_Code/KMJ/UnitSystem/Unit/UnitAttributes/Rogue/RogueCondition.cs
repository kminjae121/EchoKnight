using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class RogueCondition : MonoBehaviour, IUnitCondition
    {
        private Unit _unit;
        
        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        public bool CheckCondition()
        {
            return false;
        }
    }
}