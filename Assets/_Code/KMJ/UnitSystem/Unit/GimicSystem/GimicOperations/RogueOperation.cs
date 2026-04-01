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
            target.GetComponent<UnitHealth>().ApplyDamage(_damageData, target.transform.position,
                target.transform.position,atkData, null,false);
        }

        public override void ResetOperation(GameObject target)
        {
            return;
        }
    }
}