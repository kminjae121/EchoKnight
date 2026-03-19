using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public abstract class GimicCondition : MonoBehaviour,IGimicComponent
    {
        public virtual bool CheckCondition()
        {
            return true;
        }

        public virtual bool CheckCondition(GameObject target)
        {
            return true;
        }

        public virtual void SetCondition()
        {
            
        }

        public virtual void SetCondition(GameObject target)
        {
            
        }

        public virtual void RemoveCondition()
        {
            
        }

        public virtual void RemoveCondition(GameObject target)
        {
            
        }
    }
}