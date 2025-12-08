using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.UnitSystem;
using UnityEngine;

namespace Code.Managers
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private int showFutureTurnCount = 5;
        [SerializeField] private float requiredGauge = 100f;
        [SerializeField] private UnitManager unitManager;
        
        private readonly Queue<ITurnable> _turnQueue = new();
        private ITurnable _currentTurnUnit;

        private void Awake()
        {
            Bus<UnitTurnEndEvent>.Subscribe(EndUnitTurn);
        }


        private void Start()
        {
            StartBattle();
        }

        public void StartBattle()
        {
            ChargeTurnGauge();
        }
        
        private void EndUnitTurn(UnitTurnEndEvent evt)
        {
            _currentTurnUnit?.OnTurnEnd();
            _currentTurnUnit = null;
            
            AddTurnGauge();
            UpdateTurnQueue();
            StartNextTurn();
        }
        
        private void ChargeTurnGauge()
        {
            var units =  unitManager.GetAllUnits();

            foreach (var unit in units)
                unit.TurnGauge += unit.TurnSpeed + requiredGauge;

            UpdateTurnQueue();
            StartNextTurn();
        }
        
        private void StartNextTurn()
        {
            if (_turnQueue.Count == 0)
            {
                ChargeTurnGauge();
                return;
            }

            _currentTurnUnit = _turnQueue.Dequeue();
            _currentTurnUnit?.OnTurnStart();
        }
        
        private void AddTurnGauge()
        {
            foreach (var unit in unitManager.GetAllUnits())
                unit.TurnGauge += unit.TurnSpeed;
        }

        private void UpdateTurnQueue()
        {
            var orderedUnits = unitManager.GetAllUnits()
                .Where(unit => unit.IsReadyDoAct)
                .OrderByDescending(unit => unit.TurnGauge)
                .ToList();
            
            foreach (var unit in orderedUnits)
            {
                unit.TurnGauge -= requiredGauge;
                _turnQueue.Enqueue(unit);
            }
        }
        
        #region UI Functions

        private void UpdateFutureTurnUI()
        {
        }
        
        private List<Unit> PredictFutureTurns(List<Unit> units)
        {
            // 속도 기반으로 예측해서 정렬 뒤 리턴해주기
            return null;
        }

        #endregion
    }
}