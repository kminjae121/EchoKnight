using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;

namespace Code.Managers
{
    public class UnitManager : MonoBehaviour
    {
        private readonly HashSet<Unit> activeUnits = new();

        private void Awake()
        {
            Bus<UnitSpawnEvent>.Subscribe(RegisterUnit);
            Bus<UnitDeadEvent>.Subscribe(RemoveUnit);
        }

        private void OnDestroy()
        {
            Bus<UnitSpawnEvent>.Unsubscribe(RegisterUnit);
            Bus<UnitDeadEvent>.Unsubscribe(RemoveUnit);
        }

        #region Public Functions

        public IReadOnlyCollection<Unit> GetAllUnits()
            => activeUnits;

        public IEnumerable<Unit> GetPlayerUnits()
            => activeUnits.Where(unit => unit.IsPlayerUnit);

        public IEnumerable<Unit> GetEnemyUnits()
            => activeUnits.Where(unit => !unit.IsPlayerUnit);
        
        #endregion
        
        private void RegisterUnit(UnitSpawnEvent evt)
            => activeUnits.Add(evt.Unit);

        private void RemoveUnit(UnitDeadEvent evt)
            => activeUnits.Remove(evt.Unit);
    }
}