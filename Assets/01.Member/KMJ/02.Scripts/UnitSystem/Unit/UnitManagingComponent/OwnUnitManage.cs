using System;
using System.Collections.Generic;
using Code.Core;
using GameEventChannel;
using Input;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;

namespace UnitManaging
{
    public class OwnUnitManage : MonoSingleton<OwnUnitManage>
    {
        //[SerializeField] private GameEventChannelSO unitDeadEventChannel;
        //[SerializeField] private UnitStorage storageCompo;
        //[field: SerializeField] public List<Transform> startingTrm { get; private set; }  
        //
        //public List<UnitInfoSO> _selectedUnits { get; private set; } = new List<UnitInfoSO>();
        //
        //private List<Unit> _myOwnUnitList = new List<Unit>();
        //
        //private Unit _currrentSelectedUnit;
        //
        //private int _currentSelectedUnitIndex;
        //
        //private void Awake()
        //{
        //    unitDeadEventChannel.AddListener<UnitDeadEvent>(RemoveDeadUnit);
        //}
        //
        //
        //private void Start()
        //{
        //    SelectUnits("Golden");
        //    SelectUnits("Light");
        //    
        //    MakeGameUnit();
        //}
        //
        //private void MakeGameUnit()
        //{
        //    if (_selectedUnits.Count == 0)
        //        return;
        //    
        //    for (int i = 0; i < _selectedUnits.Count; i++)
        //    {
        //        GameObject spawnUnit = Instantiate(_selectedUnits[i].UnitPrefab,
        //            startingTrm[i].position, Quaternion.identity);
        //
        //
        //        _myOwnUnitList.Add(spawnUnit.GetComponent<Unit>());
        //    }
        //    
        //    _myOwnUnitList.Sort((a, b) => b.unitSO.turnSpeed.CompareTo(a.unitSO.turnSpeed));
        //
        //    SelectUnit(_myOwnUnitList[0]);
        //}
        //
        //public void SelectUnit(Unit unit)
        //{
        //    if (unit == _currrentSelectedUnit)
        //    {
        //        Debug.Log("원래 선택된 친구");
        //        return;
        //    }
        //    
        //    ChangingSelectUnit(unit.name);
        //}
        //
        //public void NextTurnUnit()
        //{
        //    if (_currentSelectedUnitIndex > _selectedUnits.Count - 1)
        //    {
        //        _currentSelectedUnitIndex = 0;
        //    }
        //    
        //    SelectUnit(_myOwnUnitList[_currentSelectedUnitIndex]);
        //}
        //
        //private void ChangingSelectUnit(string UnitName)
        //{
        //    if (_currrentSelectedUnit != null)
        //    {
        //        _currrentSelectedUnit.GetComponent<BasicUnit>().SelectThisUnit(false);   
        //    }
        //    
        //    Unit unit = _myOwnUnitList.Find(unit => unit.gameObject.name == UnitName);
        //
        //    _currrentSelectedUnit = unit;
        //
        //    _currrentSelectedUnit.GetComponent<BasicUnit>().SelectThisUnit(true);
        //}
        //
        //private void ChangingSelectUnit(int unitIndex)
        //{
        //    _currrentSelectedUnit.GetComponent<BasicUnit>().SelectThisUnit(false);
        //    
        //    _currrentSelectedUnit = _myOwnUnitList[unitIndex];
        //
        //    _currrentSelectedUnit.GetComponent<BasicUnit>().SelectThisUnit(true);
        //}
        //
        ///// <summary>
        ///// 유닛을 선택하는 코드
        ///// </summary>
        ///// <param name="selectedUnits"></param>
        //public void SelectUnits(string selectedUnits)
        //{
        //    _selectedUnits.Add(storageCompo.GetUnitInfo(selectedUnits));
        //}
        //
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
