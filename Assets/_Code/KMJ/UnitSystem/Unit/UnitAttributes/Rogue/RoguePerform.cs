using Code.Core.Events.Bus;
using Code.UnitSystem.Combat;
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

        public void Perform(GameObject target)
        {
            DamageData data = new DamageData();

            data.damage = 44444444;
            
            Bus<DamageEvent>.Raise(new DamageEvent(data,target,0, _unit,false,false,0.3f));
        }
    }
}