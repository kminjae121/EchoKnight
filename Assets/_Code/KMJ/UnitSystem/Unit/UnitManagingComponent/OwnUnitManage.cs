using System.Collections.Generic;
using _Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.UnitSystem;
using GameEventChannel;
using UnitSystem;
using UnityEngine;

namespace Code.UnitManaging
{
    public class OwnUnitManage : MonoBehaviour
    {
        public static OwnUnitManage Instance { get; private set; }

        [SerializeField] private GameEventChannelSO unitDeadEventChannel;
        [SerializeField] private UnitStorage storageCompo;
        [field: SerializeField] public List<Transform> startingTrm { get; private set; }
        
        public float currentCost { get; set; }

        public List<UnitSpawnSO> _selectedUnits { get; private set; } = new List<UnitSpawnSO>();

        private readonly List<Unit> _myOwnUnitList = new List<Unit>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            currentCost = 100;
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
                if (i >= 3)
                    return;

                GameObject spawnUnit = Instantiate(
                    _selectedUnits[i].UnitPrefab,
                    startingTrm[i].position,
                    Quaternion.identity
                );

                startingTrm[i].GetComponent<IMapTile>().SetObstacle(true);

                Unit unit = spawnUnit.GetComponent<Unit>();

                Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(unit));
                _myOwnUnitList.Add(unit);

                if (unit is BasicUnit basicUnit)
                {
                    basicUnit._startTile = startingTrm[i].gameObject;

                    count += 1;
                    basicUnit.PlayableUnitID = count;

                    Bus<SetUpUnitHealthBar>.Raise(new SetUpUnitHealthBar(
                        basicUnit.PlayableUnitID,
                        1, 1,
                        basicUnit.UnitImage
                    ));

                    StageManager.Instance.AddPlayerCnt();
                }
            }
        }

        /// <summary>
        /// 유닛을 선택하는 코드
        /// </summary>
        public void SelectUnits()
        {
            _selectedUnits.Clear();
            storageCompo.unitInfos.ForEach(unit => _selectedUnits.Add(unit));
        }
    }
}
