
namespace Code.Core.Interfaces
{
    public interface ITurnable
    {
        bool IsPlayerUnit { get; }

        float TurnGauge { get; }
        
        bool IsReadyDoAct { get; }
        
        float TurnSpeed { get; }
    }
}