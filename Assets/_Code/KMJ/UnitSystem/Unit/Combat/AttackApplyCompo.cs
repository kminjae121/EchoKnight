using System;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.GimicSystem;
using UnityEngine;

namespace Code.UnitSystem.Combat
{
    public class AttackApplyCompo : MonoBehaviour, IUnitComponent
    {
        private Unit _owner;
        
        public void Initialize(Unit owner)
        {
            _owner = owner;
            
            Bus<DamageEvent>.Subscribe(GetApplyDamage);
        }

        private void OnDisable()
        { 
            Bus<DamageEvent>.Unsubscribe(GetApplyDamage);
        }

        public void GetApplyDamage(DamageEvent evt)
        {
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            
            //if(evt.target.GetComponent<markComponent>().isMarking == true || _owner.unitSO.UnitType == UnitType.Bandlt)
            //    Bus<UnitGimicEvent>.Raise(new UnitGimicEvent(UnitType.Bandlt, evt.target));
            
            
            
            if(_owner.unitSO.UnitType == UnitType.Knight)
                Bus<UnitGimicEvent>.Raise(new UnitGimicEvent(UnitType.Knight));
            
            evt.target.GetComponent<IDamageable>().ApplyDamage(evt.DamageData, evt.target.transform.position,
                evt.target.transform.position, evt.atkData, _owner);
        }
    }
}