using Code.Core.Interfaces;

namespace Code.Core.Events.Bus
{
    public class CombatUnitHoverEvent : IEvent
    {
        public ITurnable HoveredUnit { get; }
        public bool IsHoverEnter { get; }

        public CombatUnitHoverEvent(ITurnable hoveredUnit, bool isHoverEnter)
        {
            HoveredUnit = hoveredUnit;
            IsHoverEnter = isHoverEnter;
        }
    }
}