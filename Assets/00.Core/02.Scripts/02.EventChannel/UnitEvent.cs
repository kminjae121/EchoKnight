using UnitSystem;
using UnityEngine;

namespace GameEventChannel
{
    public static class UnitEvent
    {
        public static UnitSelectEvent UnitSelectEvent = new UnitSelectEvent();
        public static UnitDeadEvent UnitDeadEvent = new UnitDeadEvent();
    }

    public class UnitSelectEvent : GameEvent
    {
        public Unit Unit;

        public UnitSelectEvent Initializer(Unit unit)
        {
            Unit = unit;
            return this;
        }
    }

    public class UnitDeadEvent : GameEvent
    {
        public string DeadUnitName;

        public UnitDeadEvent Initializer(string UnitName)
        {
            DeadUnitName = UnitName;
            return this;
        }
    }
}