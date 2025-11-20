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
        public GameObject Unit;

        public UnitSelectEvent Initializer(GameObject unit)
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