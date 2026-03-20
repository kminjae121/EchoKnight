using System.Collections.Generic;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public class RogueCondition : GimicCondition
    {
        private readonly Dictionary<GameObject, int> _markDictionary = new Dictionary<GameObject, int>();
        public override bool CheckCondition(GameObject target)
        {
            if (_markDictionary[target] >= 5)
            {
                Debug.Log(_markDictionary[target]);
                return true;
            }
            return false;
        }

        public override void SetCondition(GameObject target)
        {
            if (_markDictionary.TryGetValue(target, out var count))
                _markDictionary[target] = count + 1;
            else
                _markDictionary[target] = 1;
            
            
            Bus<SetMarkEvent>.Raise(new SetMarkEvent(target));
        }

        public override void RemoveCondition(GameObject target)
        {
            if (!target) return;

            _markDictionary.Remove(target);
        }
    }
}