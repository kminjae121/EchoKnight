using System;
using Code.Core.Events.Bus;
using Code.UI;
using Code.UnitSystem;
using GameEventChannel;
using Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace  UnitSystem
{
    public class BasicUnit : Unit
    {
        [field: SerializeField] public InputReader inputSO { get; private set; }
        
        [SerializeField] private GameEventChannelSO unitDeadChannel;

        public int maxCardCost = 10;

        private UnitControl _controlUI;

        private Button endTurnBtn;
        public int cardCost { get; private set; }
        
        

        private void Start()
        {
            _controlUI = GameObject.Find("BaseButton").GetComponent<UnitControl>();
            endTurnBtn = GameObject.Find("TurnEnd").GetComponent<Button>();
            
            
            endTurnBtn.onClick.AddListener(TurnEnd);
        }

        public override void OnTurnStart()
        {
            isMyTurn = true;
            OnStartTurnEvent?.Invoke();
            base.OnTurnStart();
        }

        public override void OnTurnEnd()
        {
                isMyTurn = false;
            
                base.OnTurnEnd();
                TurnEnd();
                _controlUI.SetMovingTrue();
                _controlUI.SetAttackingTrue();
        }

        public void TurnEnd()
        {
            if (isMyTurn)
            {
                OnEndTurnEvent?.Invoke();
                Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
            }
        }

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