using System;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using UnityEditor.Rendering;
using UnityEngine;

namespace _Code.Passive
{
    public class ThornmailPassive : AlwaysTurnPassive
    {
        private CharacterUnit _character;

        private void Awake()
        {
            _character = _unit as CharacterUnit;
        }

        public override void StartPassive()
        {
            _character.HealthCompo.OnInteractionEvent.AddListener(Thornmail);
        }

        public override void StopPassive()
        {
            _character.HealthCompo.OnInteractionEvent.RemoveListener(Thornmail);
        }

        private void Thornmail(Unit target, int value)
        {
            DamageData damageData = new DamageData();
            damageData.damage = (int)(value * 0.3f);
            target.GetComponent<IDamageable>().ApplyDamage(damageData, target.transform.position,
                transform.transform.position, null, _character, false, false);
        }
    }
}