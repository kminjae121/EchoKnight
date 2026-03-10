using System.Collections.Generic;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitManageRangeCompo : MonoBehaviour, IUnitComponent
    {
        private List<RangeComponent> _rangeComponents = new List<RangeComponent>();
        
        public void Initialize(Unit owner)
        {
            GetComponentsInChildren<RangeComponent>().ToList().ForEach(compo =>
            {
                _rangeComponents.Add(compo);
            });
        }

        public void RemoveAllRange()
        {
            _rangeComponents.ForEach(compo =>
            {
                compo.ResetTile();
                compo.EndAct();
            });
        }
    }
}