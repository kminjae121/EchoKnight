using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
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

        private void OnDestroy()
        {
            Bus<UnitTurnEndEvent>.Unsubscribe(EndUnitTurn);
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

        /// <summary>
        /// 처음에 모든 유닛을 행동 가능하도록 만드는 함수 ㅋㅋㄹㅃㅃ
        /// </summary>
        private void ChargeTurnGauge()
        {
            var units = unitManager.GetAllUnits();

            foreach (var unit in units)
                unit.TurnGauge += unit.TurnSpeed + requiredGauge;

            UpdateTurnQueue();
            StartNextTurn();
        }

        private void StartNextTurn()
        {
            while (_turnQueue.Count == 0)
            {
                AddTurnGauge();
                UpdateTurnQueue();
            }

            _currentTurnUnit = _turnQueue.Dequeue();
            _currentTurnUnit?.OnTurnStart();
        }

        private void AddTurnGauge()
        {
            foreach (var unit in unitManager.GetAllUnits())
                unit.TurnGauge += unit.TurnSpeed;
        }

        /// <summary>
        /// 유닛을 턴 큐에 넣는다.
        /// </summary>
        private void UpdateTurnQueue()
        {
            var orderedUnits = unitManager.GetAllUnits()
                .Where(unit => unit.IsReadyDoAct)
                .OrderByDescending(unit => unit.TurnGauge)
                .ToList();

            foreach (var unit in orderedUnits)
                while (unit.TurnGauge >= requiredGauge) // 연속 행동이 가능하게
                {
                    unit.TurnGauge -= requiredGauge;
                    _turnQueue.Enqueue(unit);
                }
        }

        #region UI Function

        private List<ITurnable> GetFutureTurns()
        {
            // 나중에 턴 큐에 없는 애들도 줘야 함.
            return _turnQueue.ToList();
        }

        #endregion
    }
}