using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Debugs;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using GondrLib.Dependencies;
using UnityEngine;

namespace Code.Managers
{
    [Provide]
    public class TurnManager : MonoBehaviour, IDependencyProvider
    {
        [Header("Turn Settings")]
        [SerializeField] private float baseTurnGauge = 100f;
        [SerializeField] private float firstRoundInterval = 150f;
        [SerializeField] private float roundInterval = 100f;

        [Header("Dependencies")]
        [SerializeField] private UnitManager unitManager;

        public int CurrentRound { get; private set; }

        public event Action OnTurnStart;

        private ITurnable _currentTurnUnit;
        private List<ITurnable> _units;
        private RoundTracker _roundTracker;
        private bool _turnFlag;

        private void Awake()
        {
            Bus<UnitTurnEndEvent>.Subscribe(OnUnitTurnEnd);
        }

        private void Update()
        {
            if (!_turnFlag)
                return;
            
            _turnFlag = false;
            StartNextTurn();
        }

        private void OnDestroy()
        {
            Bus<UnitTurnEndEvent>.Unsubscribe(OnUnitTurnEnd);
        }

        public void StartBattle()
        {
            CurrentRound = 1;
            
            _roundTracker = new RoundTracker();
            _roundTracker.NextRound = 2;
            _roundTracker.TurnGauge = firstRoundInterval;

            RefreshUnits();

            foreach (var unit in _units)
            {
                if (unit is RoundTracker) continue;
                unit.TurnGauge = CalculateBaseTurnGauge(unit);
            }

            StartNextTurn();
        }

        private float CalculateBaseTurnGauge(ITurnable unit)
        {
            return baseTurnGauge / Mathf.Max(1f, unit.TurnSpeed);
        }

        private void OnUnitTurnEnd(UnitTurnEndEvent evt)
        {
            if (_currentTurnUnit == null) return;
            
            _currentTurnUnit.TurnGauge = CalculateBaseTurnGauge(_currentTurnUnit);
            _currentTurnUnit = null;

            _turnFlag = true;
        }

        private void StartNextTurn()
        {
            int safeCount = 0;
            while (safeCount < 100)
            {
                safeCount++;
                RefreshUnits();

                _currentTurnUnit = GetNextUnit();
                AdvanceTime(_currentTurnUnit);

                if (_currentTurnUnit is RoundTracker rt)
                {
                    CurrentRound = rt.NextRound;
                    rt.NextRound = CurrentRound + 1;
                    rt.TurnGauge = roundInterval;
                    _currentTurnUnit = null;
                    
                    Bus<TurnOrderUpdateEvent>.Raise(new TurnOrderUpdateEvent());
                    continue;
                }

                OnTurnStart?.Invoke();
                _currentTurnUnit.OnTurnStart();

                Bus<TurnOrderUpdateEvent>.Raise(new TurnOrderUpdateEvent());
                return;
            }
            
            UnityLogger.LogError("턴을 계산하는 과정에서 무한 루프가 발생했습니다.");
        }

        private void RefreshUnits()
        {
            _units = unitManager.GetAllUnits().OfType<ITurnable>().ToList();

            if (_roundTracker != null)
                _units.Add(_roundTracker);
        }

        private ITurnable GetNextUnit()
        {
            return _units.OrderBy(u => u.TurnGauge).First();
        }

        private void AdvanceTime(ITurnable actingUnit)
        {
            float delta = actingUnit.TurnGauge;

            foreach (var unit in _units)
            {
                unit.TurnGauge -= delta;
            }

            ClampAllTurnGauge();
        }

        private void ClampAllTurnGauge()
        {
            foreach (var unit in _units)
            {
                unit.TurnGauge = Mathf.Max(0f, unit.TurnGauge);
            }
        }

        public void ModifyTurnGauge(ITurnable unit, float delta)
        {
            unit.TurnGauge += delta;
            unit.TurnGauge = Mathf.Max(0f, unit.TurnGauge);
            Bus<TurnOrderUpdateEvent>.Raise(new TurnOrderUpdateEvent());
        }

        public void ForceImmediateTurn(ITurnable unit)
        {
            unit.TurnGauge = 0f;
            Bus<TurnOrderUpdateEvent>.Raise(new TurnOrderUpdateEvent());
        }

        public List<ITurnable> GetTimelineUnits(int count)
        {
            List<ITurnable> timeline = new List<ITurnable>();
            if (_units == null || _units.Count == 0) return timeline;

            Dictionary<ITurnable, float> currentGauges = new Dictionary<ITurnable, float>();
            foreach (var u in _units)
            {
                currentGauges[u] = u.TurnGauge;
            }

            for (int i = 0; i < count; i++)
            {
                if (currentGauges.Count == 0) break;

                ITurnable nextUnit = null;
                float minGauge = float.MaxValue;

                foreach (var kvp in currentGauges)
                {
                    if (kvp.Value < minGauge)
                    {
                        minGauge = kvp.Value;
                        nextUnit = kvp.Key;
                    }
                }

                if (nextUnit == null) break;

                timeline.Add(nextUnit);

                var keys = currentGauges.Keys.ToList();
                foreach (var k in keys)
                {
                    currentGauges[k] -= minGauge;
                }

                if (nextUnit is RoundTracker)
                {
                    currentGauges[nextUnit] += roundInterval;
                }
                else
                {
                    currentGauges[nextUnit] += CalculateBaseTurnGauge(nextUnit);
                }
            }

            return timeline;
        }
    }
}