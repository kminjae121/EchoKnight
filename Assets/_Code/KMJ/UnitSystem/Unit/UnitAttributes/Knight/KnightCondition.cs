using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class KnightCondition : MonoBehaviour, IUnitCondition
    {
        private Unit _unit;
        private int _stack = 0;
        [SerializeField] private int _maxStack;

        public void Initialize(Unit unit)
        {
            _unit = unit;
        }
        
        private void SetStack()
        {
            _stack += 1;
        }

        public bool CheckCondition(GameObject unit)
        {
            SetStack();

            if (_stack >= _maxStack)
            {
                _stack = 0;
                return true;
            }
            
            return false;
        }
    }
}