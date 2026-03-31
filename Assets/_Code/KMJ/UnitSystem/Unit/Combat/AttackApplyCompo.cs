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
            
                if (evt.Owner as CharacterUnit)
                {
                    KnightEvent(evt);
                    RogueEvent(evt);
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

        private static void RogueEvent(DamageEvent evt)
        {
            CharacterUnit unit = evt.Owner as CharacterUnit;
            
            if (evt.target.TryGetComponent(out MarkComponent mark))
            {
                if (mark.isMarking == true)
                {
                    Bus<UnitGimicEvent>.Raise(new UnitGimicEvent(UnitType.Bandlt, evt.target, GimicOption.TargetGimic));
                    return;
                }
            }

            if ((evt.Owner.unitSO.UnitType == UnitType.Bandlt && evt.addDamage != 0) || unit.IsConfirmationSkill)
                Bus<UnitGimicEvent>.Raise(new UnitGimicEvent(UnitType.Bandlt, evt.target,GimicOption.TargetGimic));
        }

        private static void KnightEvent(DamageEvent evt)
        {
            if (evt.Owner.unitSO.UnitType == UnitType.Knight && evt.isUseOwnGimic)
                Bus<UnitGimicEvent>.Raise(new UnitGimicEvent(UnitType.Knight, null, GimicOption.OwnGimic));
        }
    }
}