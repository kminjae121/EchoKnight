using _Code.UnitSystem;
using Code.Core.Events.Bus;
using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class RoguePerform : MonoBehaviour, IUnitPerform
    {
        private UnitEffectCompo _effectCompo;
        private Unit _unit;
        
        public void Initialize(Unit unit)
        {
            _unit = unit;

            _effectCompo = _unit.GetUnitCompo<UnitEffectCompo>();
        }

        public void Perform(GameObject target)
        {
            Vector3 position = target.GetComponentInChildren<UnitAnimation>().gameObject.transform.position;
            _effectCompo.PlayTargetEffect("DarkDie", position);
            
            DamageData data = new DamageData();

            data.damage = 44444444;
            
            Bus<DamageEvent>.Raise(new DamageEvent(data,target,0, _unit,false,false,0.3f));
        }
    }
}