using System.Collections.Generic;
using System.Linq;
using Code.Core.Interfaces;
using Code.UnitSystem;
using UnitSystem;
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

        private void AddTurnGauge()
        {
            var units =  unitManager.GetAllUnits();
            
            foreach (var unit in units)
                unit.TurnGauge += unit.TurnSpeed;
            
            var orderedUnits = units
                .Where(unit => unit.IsReadyDoAct)
                .OrderByDescending(unit => unit.TurnGauge)
                .ToList();

            foreach (var unit in orderedUnits)
            {
                unit.TurnGauge -= requiredGauge;
                _turnQueue.Enqueue(unit);
            }
        }

        private void SortedTurnQueue()
        {
            var orderedUnits = unitManager.GetAllUnits()
                .Where(unit => unit.IsReadyDoAct)
                .OrderByDescending(unit => unit.TurnGauge)
                .ToList();
        }
        
        public void StartBattle()
        {
            StartTurn();
        }
        
        private void StartTurn()
        {
            var units =  unitManager.GetAllUnits();

            foreach (var unit in units)
            {
                unit.TurnGauge += unit.TurnSpeed + requiredGauge;
            }

            var orderedUnits = units
                .Where(unit => unit.IsReadyDoAct)
                .OrderByDescending(unit => unit.TurnGauge)
                .ToList();

            foreach (var unit in orderedUnits)
            {
                unit.TurnGauge -= requiredGauge;
                _turnQueue.Enqueue(unit);
            }

            StartNextUnitTurn();
        }

        private void StartNextUnitTurn()
        {
            if (_turnQueue.Count == 0)
            {
                StartTurn();
                return;
            }

            _currentTurnUnit = _turnQueue.Dequeue();
            
            //_currentTurnUnit.OnTurnStart();
        }

        private void EndUnitTurn()
        {
            AddTurnGauge();
            StartNextUnitTurn();
        }
        
        private void UpdateFutureTurnUI()
        {
            
        }
        
        private List<Unit> PredictFutureTurns(List<Unit> units)
        {
            // 속도 기반으로 예측해서 정렬 뒤 리턴해주기
            
            return null;
        }
    }
}