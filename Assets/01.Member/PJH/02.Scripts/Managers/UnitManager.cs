using System.Collections.Generic;
using Code.Core.Events.Bus;
using UnitSystem;
using UnityEngine;

namespace Code.Managers
{
    public class UnitManager : MonoBehaviour
    {
        public readonly HashSet<Unit> activeUnits = new();

        private void OnEnable()
        {
            Bus<UnitSpawnEvent>.Subscribe(RegisterUnit);
            Bus<UnitDeadEvent>.Subscribe(RemoveUnit);
        }

        private void OnDisable()
        {
            Bus<UnitSpawnEvent>.Unsubscribe(RegisterUnit);
            Bus<UnitDeadEvent>.Unsubscribe(RemoveUnit);
        }

        private void RegisterUnit(UnitSpawnEvent evt)
            => activeUnits.Add(evt.Unit);

        private void RemoveUnit(UnitDeadEvent evt)
            => activeUnits.Remove(evt.Unit);
    }
}