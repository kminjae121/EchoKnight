using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    
    public abstract class GimicOperation : MonoBehaviour, IGimicComponent
    {
        
        protected SkillComponent _skillCompo;

        public void InitializeOperation(SkillComponent skillCompo)
        {
            this._skillCompo = skillCompo;
        }
        
        public virtual void StartOperation()
        {
            
        }

        public virtual void ResetOperation()
        {
            
        }

        public virtual void ResetOperation(GameObject target)
        {
            
        }

        public virtual void StartOperation(GameObject target)
        {
            
        }
    }
}