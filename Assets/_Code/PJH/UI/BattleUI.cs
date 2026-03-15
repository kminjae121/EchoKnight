using System;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UI
{
    public class BattleUI : MonoBehaviour
    {
        private void Awake()
        {
            Bus<SetAtkUIEvent>.Subscribe(SetAttackUI);
            Bus<SkillUIEvent>.Subscribe(SetSkillUI);
        }

        private void OnDestroy()
        {
            Bus<SkillUIEvent>.Unsubscribe(SetSkillUI);
            Bus<SetAtkUIEvent>.Unsubscribe(SetAttackUI);
        }

        private void SetAttackUI(SetAtkUIEvent evt)
        {
            
        }
        
        private void SetSkillUI(SkillUIEvent evt)
        {
            
        }
    }
}