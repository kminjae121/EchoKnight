using System;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.GimicSystem;
using UnityEngine;

namespace Code.UnitSystem.Combat
{
    public class AttackApplyCompo : MonoBehaviour
    {
        private void Start()
        {
            Bus<DamageEvent>.Subscribe(GetApplyDamage);
        }

        private void OnDisable()
        { 
            Bus<DamageEvent>.Unsubscribe(GetApplyDamage);
        }

        public void GetApplyDamage(DamageEvent evt)
        {
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));

            if (evt.target.GetComponent<markComponent>().isMarking == true ||
                (evt.Owner.unitSO.UnitType == UnitType.Bandlt && evt.addDamage != 0))
            {
                Bus<UnitGimicEvent>.Raise(new UnitGimicEvent(UnitType.Bandlt, evt.target,GimicOption.TargetGimic));
            }
            
            if(evt.Owner.unitSO.UnitType == UnitType.Knight && evt.isUseOwnGimic)
                Bus<UnitGimicEvent>.Raise(new UnitGimicEvent(UnitType.Knight,null,GimicOption.OwnGimic));
            
            evt.target.GetComponent<UnitHealth>().ApplyDamage(evt.DamageData, evt.target.transform.position,
                evt.target.transform.position, evt.atkData, evt.Owner);
        }
    }
}