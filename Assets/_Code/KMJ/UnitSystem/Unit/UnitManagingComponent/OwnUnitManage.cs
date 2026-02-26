using System.Collections.Generic;
using _Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Map;
using Code.UnitSystem;
using GameEventChannel;
using UnitSystem;
using UnityEngine;

namespace Code.UnitManaging
{
    public class OwnUnitManage : MonoBehaviour
    {
        public static OwnUnitManage Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameEventChannelSO unitDeadEventChannel;
        [SerializeField] private UnitStorage storageCompo;
        [SerializeField] private GridMap gridMap;

        [Header("Spawn Settings")]
        [SerializeField] public List<Vector2Int> startingCoords = new List<Vector2Int>();
        
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
            if (gridMap == null)
                gridMap = FindObjectOfType<GridMap>();

            currentCost = 100;
            SelectUnits();
            MakeGameUnit();
        }

        private void MakeGameUnit()
        {
            if (_selectedUnits.Count == 0)
                return;
            
            if (gridMap == null)
            {
                Debug.LogError("GridMap이 할당되지 않았습니다.");
                return;
            }

            int count = -1;

            int spawnCount = Mathf.Min(_selectedUnits.Count, startingCoords.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                if (i >= 3) return;

                Vector2Int coord = startingCoords[i];
                IMapTile tile = gridMap.GetTile(coord);

                if (tile == null)
                {
                    Debug.LogWarning($"스폰 좌표 {coord}가 유효하지 않습니다.");
                    continue;
                }

                Vector3 spawnPos = gridMap.GridToWorldPosition(coord.x, coord.y);

                GameObject spawnUnit = Instantiate(
                    _selectedUnits[i].UnitPrefab,
                    spawnPos,
                    Quaternion.identity
                );

                tile.SetObstacle(true);

                Unit unit = spawnUnit.GetComponent<Unit>();

                Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(unit));
                _myOwnUnitList.Add(unit);

                if (unit is BasicUnit basicUnit)
                {
                    if (tile is MonoBehaviour tileMono)
                    {
                        basicUnit._startTile = tileMono.gameObject;
                    }
                    

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
        
        public void SelectUnits()
        {
            _selectedUnits.Clear();
            storageCompo.unitInfos.ForEach(unit => _selectedUnits.Add(unit));
        }
    }
}