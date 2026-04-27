using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class MagicianPerform : MonoBehaviour, IUnitPerform
    {
        private Unit _unit;
        
        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        public void Perform()
        {
            
        }
    }
}