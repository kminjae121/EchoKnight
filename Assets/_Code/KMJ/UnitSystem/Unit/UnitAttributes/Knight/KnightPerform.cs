using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class KnightPerform : MonoBehaviour , IUnitPerform
    {
        [SerializeField] private KnightDefenseRange defenseCompo;
        private Unit _unit;
        
        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        public void Perform(Unit target)
        {
            foreach (var unit in defenseCompo.Targets)
            {
                unit.GetUnitCompo<UnitHealth>().IsInvincibility = true;
            }
        }
    }
}