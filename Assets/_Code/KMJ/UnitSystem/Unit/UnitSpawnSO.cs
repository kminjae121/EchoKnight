using GondrLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.UnitSystem
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Unit/UnitInfo", order = 0)]
    public class UnitSpawnSO : ScriptableObject
    {
        public string UnitName;
        public GameObject UnitPrefab;
        public PoolingItemSO poolingItem;
    }
}