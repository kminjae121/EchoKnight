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

        public void Perform(GameObject target)
        {
            foreach (var unit in defenseCompo.Targets)
            {
                unit.GetUnitCompo<InvincibilityCompo>().SetUnitInvincibility(3);
            }
        }
    }
}