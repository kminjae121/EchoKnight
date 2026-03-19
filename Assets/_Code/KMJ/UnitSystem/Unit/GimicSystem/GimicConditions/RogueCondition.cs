using System.Collections.Generic;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public class RogueCondition : GimicCondition
    {
        private Dictionary<GameObject, int> _markDictionary = new Dictionary<GameObject, int>();

        public override bool CheckCondition(GameObject target)
        {
            if (_markDictionary.GetValueOrDefault(target) == 5)
                return true;

            return false;
        }

        public override void SetCondition(GameObject target)
        {
            if (_markDictionary.ContainsKey(target))
                _markDictionary[target] += 1;
            else
                _markDictionary.Add(target, 1);
            
            Bus<SetMarkEvent>.Raise(new SetMarkEvent(target));
        }
        
        public override void RemoveCondition(GameObject target)
        {
            _markDictionary.Remove(target);
        }
    }
}