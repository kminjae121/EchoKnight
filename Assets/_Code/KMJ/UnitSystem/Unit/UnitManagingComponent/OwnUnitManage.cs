using System;
using System.Collections.Generic;
using System.Linq;
using _Code.Core.Managers;
using Code.Core;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.UnitSystem;
using GameEventChannel;
using UnitSystem;
using UnityEngine;


namespace Code.UnitManaging
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
            SelectUnits();
            MakeGameUnit();
        }
        
        private void MakeGameUnit()
        {
            if (_selectedUnits.Count == 0)
                return;

            int count = -1;
            
            for (int i = 0; i < _selectedUnits.Count; i++)
            {
                GameObject spawnUnit = Instantiate(_selectedUnits[i].UnitPrefab,
                    startingTrm[i].position, Quaternion.identity);

                startingTrm[i].GetComponent<IMapTile>().SetObstacle(true);
                
                Unit unit = spawnUnit.GetComponent<Unit>();
        
                Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(unit));
                _myOwnUnitList.Add(unit);

                BasicUnit basicUnit = unit as BasicUnit;

                basicUnit._startTile = startingTrm[i].gameObject;

                count += 1;
                
                basicUnit.PlayableUnitID = count;
                
                Bus<SetUpUnitHealthBar>.Raise(new SetUpUnitHealthBar(basicUnit.PlayableUnitID,
                    1,1,basicUnit.UnitImage));
                
                StageManager.Instance.AddPlayerCnt();
            }
        }
        
        /// <summary>
        /// 유닛을 선택하는 코드
        /// </summary>
        /// <param name="selectedUnits"></param>
        public void SelectUnits()
        {
            storageCompo.units.Values.ToList().ForEach(unit =>
            {
                _selectedUnits.Add(unit);
            });
        }
    }
}
