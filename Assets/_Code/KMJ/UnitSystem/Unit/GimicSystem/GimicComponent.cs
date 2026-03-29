using System;
using Code.Core.Events.Bus;
using Code.SkillSystem;
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
        
        [SerializeField] private GimicCondition condition;
        [SerializeField] private GimicOperation operation;
        [SerializeField] private GimicEventComponent eventCompo;
        
        private UnitType _unitType;

        public void Initialize(Unit owner)
        {
            UnitSkillComponent skillCompo = owner.GetUnitCompo<UnitSkillComponent>();
            
            Bus<UnitGimicEvent>.Subscribe(SetCondition);
            Bus<UseGimicEvent>.Subscribe(UseCondition);
            operation.InitializeOperation(skillCompo);

            _unitType = owner.unitSO.UnitType;
        }

        private void OnDestroy()
        {
            Bus<UnitGimicEvent>.Unsubscribe(SetCondition);
            Bus<UseGimicEvent>.Unsubscribe(UseCondition);
        }

        public void SetCondition(UnitGimicEvent evt)
        {
            if (_unitType == evt.unitType)
            {
                if (evt.gimicOption == GimicOption.OwnGimic)
                {
                    condition.SetCondition();
                
                    if (condition.CheckCondition())
                        operation.StartOperation();
                }
                else if (evt.gimicOption == GimicOption.TargetGimic)
                {
                    condition.SetCondition(evt.target);

                    if (condition.CheckCondition(evt.target))
                        operation.StartOperation(evt.target);
                }   
            }
        }

        public void UseCondition(UseGimicEvent evt)
        {
            if (_unitType == evt.unitType)
            {
                if (GimicOption == GimicOption.OwnGimic)
                {
                    condition.RemoveCondition();
                    operation.ResetOperation();
                }
                else if (evt.unitType == _unitType && GimicOption == GimicOption.TargetGimic)
                {
                    condition.RemoveCondition(evt.target);
                    operation.ResetOperation(evt.target);
                }
            }
        }

    }
}