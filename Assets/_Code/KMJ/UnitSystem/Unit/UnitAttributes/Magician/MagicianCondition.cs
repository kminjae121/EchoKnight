using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class MagicianCondition: MonoBehaviour, IUnitCondition
    {
        private Unit _unit;

        private int _atkGauge;
        private int _healGauge;

        private int _maxGauge = 5;
        
        public void Initialize(Unit unit)
        {
            _unit = unit;
            _atkGauge = 0;
            _healGauge = 0;
        }

        public bool CheckCondition(GameObject unit)
        {
            if (unit != null)
            {
                _healGauge += 1;
                if (_healGauge >= _maxGauge)
                {
                    _healGauge = 0;
                    return true;
                }
            }
            else
            {
                _atkGauge += 1;
                if (_atkGauge >= _maxGauge)
                {
                    _atkGauge = 0;
                    return true;
                }
            }
            
            return false;
        }
    }
}