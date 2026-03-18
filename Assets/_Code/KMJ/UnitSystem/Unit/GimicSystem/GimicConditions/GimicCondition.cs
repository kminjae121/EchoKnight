using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public abstract class GimicCondition : MonoBehaviour,IGimicComponent
    {
        public virtual bool CheckCondition()
        {
            return true;
        }

        public virtual void SetCondition()
        {
            
        }

        public virtual void RemoveCondition()
        {
            
        }
    }
}