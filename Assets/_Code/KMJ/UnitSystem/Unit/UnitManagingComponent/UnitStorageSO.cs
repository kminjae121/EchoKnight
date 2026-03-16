using System.Collections.Generic;
using Code.UnitSystem;
using UnityEngine;

namespace Code.UnitManaging
{
    [CreateAssetMenu(fileName = "UnitStorage", menuName = "Unit/Storage", order = 0)]
    public class UnitStorageSO : ScriptableObject
    {
        public List<UnitSpawnSO> units = new();
        public List<UnitState> unitStates = new();
    }
}