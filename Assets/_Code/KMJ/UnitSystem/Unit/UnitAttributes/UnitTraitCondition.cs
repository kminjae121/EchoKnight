using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public abstract class UnitTraitCondition : MonoBehaviour
    {
        private Unit _unit;

        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        public abstract bool CheckCondition();
    }
}