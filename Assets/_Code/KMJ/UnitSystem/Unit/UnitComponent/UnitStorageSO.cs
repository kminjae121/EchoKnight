using System.Collections.Generic;
using UnitSystem;
using UnityEngine;

namespace Code.UnitSystem
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Unit/Storage", order = 0)]
    public class UnitStorageSO : ScriptableObject
    {
        public List<UnitInfoSO> units = new List<UnitInfoSO>();
    }
}