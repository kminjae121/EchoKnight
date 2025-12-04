using System;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using GameEventChannel;
using Input;
using Unity.Collections;
using UnityEngine;

namespace  UnitSystem
{
    public class BasicUnit : Unit, ITurnable
    {
        [SerializeField] private GameEventChannelSO unitDeadChannel;
        [field: SerializeField] public InputReader inputSO { get; private set; }

        public bool IsPlayerUnit => isPlayerUnit;
        
        public float TurnGauge => turnGauge;

        public bool IsReadyDoAct => TurnGauge >= 100f;

        public float TurnSpeed => turnSpeed;


        public int maxCardCost = 10;
        
        public int cardCost { get; set; } = 0;
        
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
            Bus<UnitDeadEvent>.Raise(new UnitDeadEvent(this));
        }

    }
   
}