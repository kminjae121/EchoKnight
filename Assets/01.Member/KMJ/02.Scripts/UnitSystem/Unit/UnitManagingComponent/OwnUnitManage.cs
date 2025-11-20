using System;
using System.Collections.Generic;
using GameEventChannel;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;

namespace UnitManaging
{
    public class OwnUnitManage : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO selectUnitEventChannel;
        [SerializeField] private GameEventChannelSO unitDeadEventChannel;
        [SerializeField] private UnitStorage storageCompo;
        [field: SerializeField] public List<Transform> startingTrm { get; private set; }  
        


        public List<UnitInfoSO> _selectedUnits { get; private set; }    
        
        private List<GameObject> _myOwnUnitList = new List<GameObject>();
        private GameObject _currrentSelectedUnit;

        private void Awake()
        {
            unitDeadEventChannel.AddListener<UnitDeadEvent>(RemoveDeadUnit);
        }


        private void Start()
        {
            for (int i = 0; i < _selectedUnits.Count; i++)
            {
                GameObject spawnUnit = Instantiate(_selectedUnits[i].UnitPrefab,
                    startingTrm[i].position, Quaternion.identity);
                    
                
                _myOwnUnitList.Add(spawnUnit);
            }

            ChangingSelectUnit(0);
        }
        
        /// <summary>
        /// 선택한 유닛을 바꾸는 코드
        /// </summary>
        /// <param name="unitIndex">바꿀 유닛의 인덱스</param>
        private void ChangingSelectUnit(int unitIndex)
        {
            _currrentSelectedUnit.GetComponent<BasicUnit>().SelectThisUnit(false);
            
            _currrentSelectedUnit = _myOwnUnitList[unitIndex];

            _currrentSelectedUnit.GetComponent<BasicUnit>().SelectThisUnit(true);
            
            selectUnitEventChannel.RaiseEvent(UnitEvent.UnitSelectEvent.Initializer(_currrentSelectedUnit));
        }
        
        /// <summary>
        /// 유닛을 선택하는 코드
        /// </summary>
        /// <param name="selectedUnits"></param>
        public void SelectUnits(string selectedUnits)
        {
            _selectedUnits.Add(storageCompo.GetUnitInfo(selectedUnits));
        }
        private void RemoveDeadUnit(UnitDeadEvent evt)
        {
            GameObject unit = _myOwnUnitList.Find(unit => unit.gameObject.name == evt.DeadUnitName);
           
            if (unit != null)
            {
                _myOwnUnitList.Remove(unit);
            }
        }

    }
}
