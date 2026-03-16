using System.Collections.Generic;
using Code.UnitSystem;
using UnityEngine;

namespace Code.UnitManaging
{
    public class UnitStorage : MonoBehaviour
    { 
        [SerializeField] private UnitStorageSO _storage;
        
        public Dictionary<string, UnitSpawnSO> units = new Dictionary<string, UnitSpawnSO>();
        
        public List<UnitSpawnSO> unitInfos = new List<UnitSpawnSO>();

        private void Awake()
        {
            _storage.units.ForEach(unit =>
            {
                unitInfos.Add(unit);
            });    
        }

        /// <summary>
        /// 유닛을 찾는 함수
        /// </summary>
        /// <param name="unitName">찾을 유닛 이름</param>
        /// <returns></returns>
        public UnitSpawnSO GetUnitInfo(string unitName)
        {
            return units.GetValueOrDefault(unitName);
        }
    }
}