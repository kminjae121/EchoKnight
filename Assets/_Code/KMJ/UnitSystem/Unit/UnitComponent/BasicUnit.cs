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

        public int maxUsingCost = 100;

        private UnitControl _controlUI;

        private Button endTurnBtn;
        public float CurrentCost { get; private set; }
        

        [SerializeField] private Image unitImage;

        private void Start()
        {
            _controlUI = GameObject.Find("BaseButton").GetComponent<UnitControl>();
            endTurnBtn = GameObject.Find("TurnEnd").GetComponent<Button>();
            
            endTurnBtn.onClick.AddListener(TurnEnd);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            endTurnBtn.onClick.RemoveListener(TurnEnd);
        }

        public override void OnTurnStart()
        {
            isMyTurn = true;
            CurrentCost = maxUsingCost;
            OnStartTurnEvent?.Invoke();
            base.OnTurnStart();
        }

        public override void OnTurnEnd()
        {
            isMyTurn = false;
            
            base.OnTurnEnd();
            TurnEnd();
            
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
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
            if (CurrentCost >= maxUsingCost || CurrentCost + cost >= maxUsingCost)
                return false;
            
            CurrentCost += cost;
            return true;
        }

        public void RemoveCost(float cost)
        {
            CurrentCost -= cost;
            
            Debug.Log(CurrentCost);
            
            if (CurrentCost <= 0)
            {
                CurrentCost = 0;
                TurnEnd();
            }
            //코스트 줄어드는중
        }


        public void SelectThisUnit(bool isSelected)
        {
            
        }

        public void Die()
        {
        }
    }
}