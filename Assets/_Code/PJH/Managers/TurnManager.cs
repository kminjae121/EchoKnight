using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using TMPro;
using UnityEngine;

namespace Code.Managers
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private float baseTurnGauge = 100f;
        [SerializeField] private UnitManager unitManager;
        [SerializeField] private TextMeshProUGUI turnUnitText;

        private ITurnable _currentTurnUnit;
        private List<ITurnable> _units;

        private void Awake()
        {
            Bus<UnitTurnEndEvent>.Subscribe(OnUnitTurnEnd);
        }

        private void OnDestroy()
        {
            Bus<UnitTurnEndEvent>.Unsubscribe(OnUnitTurnEnd);
        }

        public void StartBattle()
        {
            _units = unitManager.GetAllUnits().Select(u => u as ITurnable).ToList();

            foreach (var unit in _units)
                unit.TurnGauge = CalculateBaseTurnGauge(unit);

            StartNextTurn();
        }

        private float CalculateBaseTurnGauge(ITurnable unit)
            => baseTurnGauge / Mathf.Max(1, unit.TurnSpeed);

        private void OnUnitTurnEnd(UnitTurnEndEvent evt)
        {
            if (_currentTurnUnit == null)
                return;
            _currentTurnUnit.OnTurnEnd();
            _currentTurnUnit.TurnGauge = CalculateBaseTurnGauge(_currentTurnUnit);
            _currentTurnUnit = null;

            StartNextTurn();
        }

        private void StartNextTurn()
        {
            _currentTurnUnit = GetNextUnit();

            AdvanceTime(_currentTurnUnit);

            // 사실 필요 없지만 가독성을 위해
            _currentTurnUnit.TurnGauge = 0f;
            _currentTurnUnit.OnTurnStart();

            UpdateCurrentTurnUI();

            Bus<TurnOrderUpdateEvent>.Raise(new TurnOrderUpdateEvent());
        }

        private ITurnable GetNextUnit()
            => _units.OrderBy(u => u.TurnGauge).First();

        private void AdvanceTime(ITurnable actingUnit)
        {
            float delta = actingUnit.TurnGauge;

            foreach (var unit in _units)
                unit.TurnGauge -= delta;

            ClampAllTurnGauge();
        }
        
        private void ClampAllTurnGauge()
        {
            foreach (var unit in _units)
                unit.TurnGauge = Mathf.Max(0f, unit.TurnGauge);
        }
        
        /// <summary>
        /// 턴 조작 스킬용 함수
        /// 양수 : 지연, 음수 : 가속
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="delta"></param>
        public void ModifyTurnGauge(ITurnable unit, float delta)
        {
            unit.TurnGauge += delta;
            unit.TurnGauge = Mathf.Max(0f, unit.TurnGauge);
        }

        public void ForceImmediateTurn(ITurnable unit)
        {
            unit.TurnGauge = 0f;
        }

        #region UI Function

        private void UpdateCurrentTurnUI()
        {
            if (turnUnitText != null && _currentTurnUnit != null)
                turnUnitText.text = _currentTurnUnit.UnitName;
        }

        public List<ITurnable> GetTimelineUnits(int count)
        {
            return _units
                //.Where(u => u != _currentTurnUnit)
                .OrderBy(u => u.TurnGauge)
                .Take(count)
                .ToList();
        }

        #endregion
    }
}