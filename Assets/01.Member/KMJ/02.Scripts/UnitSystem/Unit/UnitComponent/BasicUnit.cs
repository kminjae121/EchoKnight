using Code.Core.Events.Bus;
using Code.UnitSystem;
using GameEventChannel;
using Input;
using UnityEngine;

namespace  UnitSystem
{
    public class BasicUnit : Unit
    {
        [field: SerializeField] public InputReader inputSO { get; private set; }
        
        [SerializeField] private GameEventChannelSO unitDeadChannel;

        public int maxCardCost = 10;
        
        public int cardCost { get; private set; }
        
        protected override void Dead()
        {
            base.Dead();
            Die();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private void OnValidate()
        {
            if (unitSO != null)
            {
                gameObject.name = unitSO.UnitName;
            }
        }

        public bool GetCost(int cost)
        {
            if (cardCost >= maxCardCost || cardCost + cost >= maxCardCost)
                return false;
            
            cardCost += cost;
            return true;
        }

        public void RemoveCost(int cost)
        {
            cardCost -= cost;
        }


        public void SelectThisUnit(bool isSelected)
        {
            Debug.Log($"{gameObject.name}의 선택이 {isSelected} 되었습니다");
            //isPlayerUnit = isSelected;
        }

        public void Die()
        {
        }
    }
}