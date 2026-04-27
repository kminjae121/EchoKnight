using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class RoguePerform : MonoBehaviour, IUnitPerform
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