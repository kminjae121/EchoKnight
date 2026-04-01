using System;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.GimicSystem;
using UnityEngine;
using Random = UnityEngine.Random;

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

            if (evt.target != null)
            {
                if (evt.target.TryGetComponent(out IDamageable damageable))
                {
                    bool isCritical = CalculateCritical(ref evt);

                    damageable.ApplyDamage(evt.DamageData, evt.target.transform.position,
                        evt.target.transform.position, evt.atkData, evt.Owner, isCritical);
                }
            }   
        }

        private bool CalculateCritical(ref DamageEvent evt)
        {
            bool isCritical = false;

            float damage = evt.DamageData.damage;
                    
            float criticalProbilityValue =
                Random.Range(0f, 100f);

            if (evt.Owner != null && criticalProbilityValue <= evt.Owner.unitSO.CriticalProbability)
            {
                isCritical = true;
                damage = damage * evt.Owner.unitSO.CriticalDamageIncrease;

                evt.DamageData.damage = (int)damage;
            }

            return isCritical;
        }
    }
}