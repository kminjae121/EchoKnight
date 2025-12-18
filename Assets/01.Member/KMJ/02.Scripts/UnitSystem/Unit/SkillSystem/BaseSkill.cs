using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public abstract class BaseSkill : MonoBehaviour
    {
        [SerializeField] protected SkillComponent _skillCompo;
        
        public float damage;

        public int useSkillPoint;

        public bool isCanUseSkill = false;
        public virtual void UseSkill()
        {
            if (_skillCompo.currentSkillCost - useSkillPoint < 0)
                return;
            
            _skillCompo.currentSkillCost -= useSkillPoint;
        }

        public void CanUseThisSkill()
        {
            isCanUseSkill = true;
        }
        
        public void BlockThisSkill()
        {
            isCanUseSkill = false;
        }
        
        
    }
}