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
        [field: SerializeField] public InputReader inputSO { get; private set; }

        public float turnSpeed { get; set; }
        public bool isPlayerUnit { get; set; }
        public float turnGauge { get; set; }
        public bool isSelect { get; set; } = false;

        public int maxCardCost = 10;
        
        public int cardCost { get; set; } = 0;
        
        protected override void Dead()
        {
            base.Dead();
            unitDeadChannel.RaiseEvent(UnitEvent.UnitDeadEvent.Initializer(gameObject.name));
        }

        protected override void Awake()
        {
            base.Awake();
            turnSpeed = unitSO.turnSpeed;
            isPlayerUnit = unitSO.isPlayerUnit;
            turnGauge = unitSO.turnGauge;
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
            isSelect = isSelected;
        }
    }
   
}