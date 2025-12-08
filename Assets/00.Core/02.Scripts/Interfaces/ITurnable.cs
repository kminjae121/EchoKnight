
namespace Code.Core.Interfaces
{
    public interface ITurnable
    {
        string UnitName { get; }
        bool IsPlayerUnit { get; }

        float TurnGauge { get; }
        
        bool IsReadyDoAct { get; }
        
        float TurnSpeed { get; }

        void OnTurnStart();
        void OnTurnEnd();
    }
}