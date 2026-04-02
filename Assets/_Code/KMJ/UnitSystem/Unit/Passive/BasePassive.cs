using UnityEngine;

namespace _Code.Passive
{
    public abstract class BasePassive : MonoBehaviour
    {
        public abstract void StartPassive();
        public abstract void StopPassive();

        public virtual void HandleTurnStartEvent()
        {   
        }
    }
}