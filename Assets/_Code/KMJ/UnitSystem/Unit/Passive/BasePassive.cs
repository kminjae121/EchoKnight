using Code.UnitSystem;
using UnityEngine;

namespace _Code.Passive
{
    public abstract class BasePassive : MonoBehaviour
    {
        [field:SerializeField] public PassiveType PassiveType { get; set; }
        protected Unit _unit;
        
        public abstract void StartPassive();
        public abstract void StopPassive();

        public void SetOwner(Unit owner)
        {
            _unit = owner;
        }
        
        public virtual void HandleTurnStartEvent()
        {   
        }
    }
}