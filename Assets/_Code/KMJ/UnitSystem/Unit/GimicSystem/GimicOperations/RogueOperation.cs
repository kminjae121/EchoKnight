using System;
using Code.Core.Events.Bus;
using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public class RogueOperation : GimicOperation
    {
        [SerializeField] private Unit unit;
        [SerializeField] private AttackDataSO atkData;
        private DamageData _damageData;

        private void Awake()
        {
            _damageData.damage = 999999;
        }

        public override void StartOperation(GameObject target)
        {
            Bus<DamageEvent>.Raise(new DamageEvent(_damageData,atkData,target));
        }

        public override void ResetOperation(GameObject target)
        {
            return;
        }
    }
}