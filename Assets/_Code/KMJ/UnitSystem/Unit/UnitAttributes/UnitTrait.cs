using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class UnitTrait : MonoBehaviour, IUnitComponent
    {
        private Unit _unit;
        private UnitTraitPerform perform;
        private UnitTraitCondition condition;
        
        public void Initialize(Unit owner)
        {
            _unit = owner;

            condition = GetComponentInChildren<UnitTraitCondition>();
            
            perform = GetComponentInChildren<UnitTraitPerform>();

            if (_unit != null)
            {
                condition.Initialize(_unit);
                perform.Initialize(_unit);
            }
            else
                Debug.LogWarning("유닛이 할당되어있지 않습니다.");
        }

        private void CheckCondition()
        {
            if (condition.CheckCondition())
            {
                Perform();
            }
        }

        private void Perform()
        {
            perform.PerformTrait();
        }
    }
}