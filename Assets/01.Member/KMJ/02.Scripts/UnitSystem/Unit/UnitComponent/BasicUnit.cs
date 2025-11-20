using System;
using GameEventChannel;
using Input;
using UnitSystem;
using UnityEngine;

namespace  UnitSystem
{
    public class BasicUnit : Unit
    {
        [SerializeField] private GameEventChannelSO unitDeadChannel;
        public InputReader inputSO { get; private set; }

        public bool isSelect { get; set; } = false;
        
        public int cardCost { get; set; } = 0;
        
        protected override void Dead()
        {
            base.Dead();
            unitDeadChannel.RaiseEvent(UnitEvent.UnitDeadEvent.Initializer(gameObject.name));
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
            if (cardCost >= 10 || cardCost + cost >= 10)
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
            isSelect = isSelected;
        }
    }
   
}