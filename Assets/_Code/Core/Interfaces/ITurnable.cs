
using UnityEngine;

namespace Code.Core.Interfaces
{
    public interface ITurnable
    {
        string UnitName { get; }
        bool IsPlayerUnit { get; }

        float TurnGauge { get; set; }
        
        bool IsReadyDoAct { get; }
        
        float TurnSpeed { get; }
        
        Sprite UnitImage { get; }

        void OnTurnStart();
        void OnTurnEnd();
    }
}