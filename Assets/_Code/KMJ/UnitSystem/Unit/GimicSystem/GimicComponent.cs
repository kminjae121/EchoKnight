using System;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public enum GimicOption
    {
        OwnGimic,
        TargetGimic
    }
    public class GimicComponent : MonoBehaviour, IUnitComponent
    {
        public GimicOption GimicOption;
        
        [SerializeField] private GimicCondition _condition;
        [SerializeField] private GimicOperation _operation;
        [SerializeField] private GimicEventComponent _eventCompo;
        
        private UnitType _unitType;

        public void Initialize(Unit owner)
        {
            SkillComponent skillCompo = owner.GetUnitCompo<SkillComponent>();
            
            Bus<UnitGimicEvent>.Subscribe(SetCondition);
            Bus<UseGimicEvent>.Subscribe(UseCondition);
            _operation.InitializeOperation(skillCompo);

            _unitType = owner.unitSO.UnitType;
        }

        private void OnDisable()
        {
            Bus<UnitGimicEvent>.Unsubscribe(SetCondition);
            Bus<UseGimicEvent>.Unsubscribe(UseCondition);
        }

        public void SetCondition(UnitGimicEvent evt)
        {
            if (evt.unitType == _unitType && GimicOption ==  GimicOption.OwnGimic)
            {
                _condition.SetCondition();
                
                if (_condition.CheckCondition())
                    _operation.StartOperation();
            }
            else if (evt.unitType == _unitType && GimicOption == GimicOption.TargetGimic)
            {
                _condition.SetCondition(evt.target);
                
                if(_condition.CheckCondition())
                    _operation.StartOperation(evt.target);
            }
        }

        public void UseCondition(UseGimicEvent evt)
        {
            if (evt.unitType == _unitType &&  GimicOption == GimicOption.OwnGimic)
            {
                _condition.RemoveCondition();
                _operation.ResetOperation();
            }
            else if (evt.unitType == _unitType && GimicOption == GimicOption.TargetGimic)
            {
                _condition.RemoveCondition(evt.target);
                _operation.ResetOperation(evt.target);
            }
        }

    }
}