using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.SkillSystem;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public class KnightOperation : GimicOperation
    {
        [SerializeField] private List<int> _addDamageList;
        
        private int _addDamage = 0;

        private int _operationLevel = 0;
        
        public override void StartOperation()
        {
            _addDamage = _addDamageList[_operationLevel];
            _skillCompo.SetAddSkillDamage(_addDamage,SkillType.ActiveSkill);
            _operationLevel++;
            Bus<KnightSwordEvent>.Raise(new KnightSwordEvent(_operationLevel));
        }

        public override void ResetOperation()
        {
            _addDamage = 0;
            Bus<KnightSwordEvent>.Raise(new KnightSwordEvent(0));
            _skillCompo.SetAddSkillDamage(_addDamage,SkillType.ActiveSkill);    
        }
    }
}