using System;
using System.Collections.Generic;
using Code.Core;
using Code.Core.Events.Bus;
using GameEventChannel;
using Input;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;

namespace UnitManaging
{
    public class OwnUnitManage : MonoSingleton<OwnUnitManage>
    {
        [SerializeField] private GameEventChannelSO unitDeadEventChannel;
        [SerializeField] private UnitStorage storageCompo;
        [field: SerializeField] public List<Transform> startingTrm { get; private set; }  
        
        public List<UnitInfoSO> _selectedUnits { get; private set; } = new List<UnitInfoSO>();
        
        private List<Unit> _myOwnUnitList = new List<Unit>();
        
        private void Awake()
        {
        }
        
        
        private void Start()
        {
            SelectUnits("Golden");
            SelectUnits("Light");
            MakeGameUnit();
        }
        
        private void MakeGameUnit()
        {
            if (_selectedUnits.Count == 0)
                return;
            
            for (int i = 0; i < _selectedUnits.Count; i++)
            {
                GameObject spawnUnit = Instantiate(_selectedUnits[i].UnitPrefab,
                    startingTrm[i].position, Quaternion.identity);
        
        
                Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(spawnUnit.GetComponent<Unit>()));
                _myOwnUnitList.Add(spawnUnit.GetComponent<Unit>());
                
            }
            
            _myOwnUnitList[0].SetThisUnit(true);
        }
        
        /// <summary>
        /// 유닛을 선택하는 코드
        /// </summary>
        /// <param name="selectedUnits"></param>
        public void SelectUnits(string selectedUnits)
        {
            _selectedUnits.Add(storageCompo.GetUnitInfo(selectedUnits));
        }
        
        //private void RemoveDeadUnit(UnitDeadEvent evt)
        //{
        //    Unit unit = _myOwnUnitList.Find(unit => unit.gameObject.name == evt.DeadUnitName);
        //   
        //    if (unit != null)
        //    {
        //        _myOwnUnitList.Remove(unit);
        //    }
        //}

    }
}
