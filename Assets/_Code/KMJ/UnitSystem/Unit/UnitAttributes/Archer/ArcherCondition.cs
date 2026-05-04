using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class ArcherCondition : MonoBehaviour, IUnitCondition
    {
        private Unit _unit;
        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        public bool CheckCondition(GameObject unit)
        {
            return false;
        }
    }
}