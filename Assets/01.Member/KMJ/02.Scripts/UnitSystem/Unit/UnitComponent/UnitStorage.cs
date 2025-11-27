using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitSystem
{
    public class UnitStorage : MonoBehaviour
    { 
        [SerializeField] private UnitStorageSO _storage;
        
        public Dictionary<string, UnitInfoSO> units = new Dictionary<string, UnitInfoSO>();

        private void Awake()
        {
            _storage.units.ForEach(unit =>
            {
                units.Add(unit.UnitName, unit);
            });    
        }

        /// <summary>
        /// 유닛을 찾는 함수
        /// </summary>
        /// <param name="unitName">찾을 유닛 이름</param>
        /// <returns></returns>
        public UnitInfoSO GetUnitInfo(string unitName)
        {
            return units.GetValueOrDefault(unitName);
        }
    }
}