using Code.Managers;
using Code.UnitSystem;
using UnityEngine;

namespace _Code.Passive
{
    public class HealHpPassive : BasePassive
    {
        private CharacterUnit _character; 
        private void Start()
        {
            _character = _unit as CharacterUnit;
        }
        
        public override void StartPassive()
        {
            //TurnManager.Instance.OnTurnStart += HealHp;
        }

        public override void StopPassive()
        {
            //TurnManager.Instance.OnTurnStart -= HealHp;
        }

        private void HealHp()
        {
            float lostHp = _character.HealthCompo.MaxHealth - _character.HealthCompo.CurrentHealth;

            int healHp = Mathf.FloorToInt(lostHp * 0.1f);
            _character.HealthCompo.HealHp(healHp);
        }
    }
}