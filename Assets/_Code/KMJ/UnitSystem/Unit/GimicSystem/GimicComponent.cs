using System;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public class GimicComponent : MonoBehaviour, IUnitComponent
    {
        private UnitType unitType;
        
        private GimicCondition _condition;
        private GimicOperation _operation;
        private GimicEventComponent _eventCompo;

        public void Initialize(Unit owner)
        {
            _condition = GetComponent<GimicCondition>();
            _operation = GetComponent<GimicOperation>();
            _eventCompo = GetComponent<GimicEventComponent>();
            SkillComponent skillCompo = owner.GetUnitCompo<SkillComponent>();
            
            Bus<UnitGimicEvent>.Subscribe(SetCondition);
            Bus<UseGimicEvent>.Subscribe(UseCondition);
            _operation.InitializeOperation(skillCompo);

            unitType = owner.unitSO.UnitType;
        }

        private void OnDisable()
        {
            Bus<UnitGimicEvent>.Unsubscribe(SetCondition);
            Bus<UseGimicEvent>.Unsubscribe(UseCondition);
        }

        public void SetCondition(UnitGimicEvent evt)
        {
            if (evt.unitType == unitType)
            {
                _condition.SetCondition();
                
                if (_condition.CheckCondition())
                    _operation.StartOperation();
            }
        }

        public void UseCondition(UseGimicEvent evt)
        {
            if (evt.unitType == unitType)
            {
                _condition.RemoveCondition();
            }
        }

    }
}