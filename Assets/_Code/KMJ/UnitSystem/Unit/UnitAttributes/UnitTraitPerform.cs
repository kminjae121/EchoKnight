using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public abstract class UnitTraitPerform : MonoBehaviour
    {
        private Unit _unit;

        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        public virtual void PerformTrait()
        {
            
        }
    }
}