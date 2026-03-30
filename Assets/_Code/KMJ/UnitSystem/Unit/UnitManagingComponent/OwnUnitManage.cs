using System.Collections.Generic;
using Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Managers;
using Code.Map;
using Code.UnitSystem;
using GameEventChannel;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.Runtime;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UnitManaging
{
    public class OwnUnitManage : MonoBehaviour
    {
        public static OwnUnitManage Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameEventChannelSO unitDeadEventChannel;
        [SerializeField] private UnitStorage storageCompo;

        [Header("Spawn Settings")]
        [SerializeField] public List<Vector2Int> startingCoords = new List<Vector2Int>();

        public List<PoolingItemSO> _selectedUnits { get; private set; } = new List<PoolingItemSO>();

        private readonly List<Unit> _myOwnUnitList = new List<Unit>();

        [SerializeField] private TurnCostGaugeManager GaugeManager;
        [SerializeField] private Button endTurnBtn;

        [Inject] private PoolManagerMono _poolManager;
        

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
            SelectUnits();
            MakeGameUnit();
        }

        private void MakeGameUnit()
        {
            if (_selectedUnits.Count == 0)
                return;

            int count = -1;

            int spawnCount = Mathf.Min(_selectedUnits.Count, startingCoords.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                if (i >= 3) return;

                Vector2Int coord = startingCoords[i];
                IMapTile tile = GridMap.Instance.GetTile(coord);

                if (tile == null)
                {
                    Debug.LogWarning($"스폰 좌표 {coord}가 유효하지 않습니다.");
                    continue;
                }

                Vector3 spawnPos = GridMap.Instance.GridToWorldPosition(coord.x, coord.y);

                GameObject spawnUnit = _poolManager.Pop<Unit>(_selectedUnits[i]).gameObject;


                spawnUnit.transform.position = spawnPos;
                
                spawnUnit.transform.rotation = Quaternion.identity;
                

                tile.SetState(TileState.Obstacle, true);

                Unit unit = spawnUnit.GetComponent<Unit>();

                Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(unit));
                _myOwnUnitList.Add(unit);

                if (unit is CharacterUnit basicUnit)
                {
                    if (tile is MonoBehaviour tileMono)
                        basicUnit._startTile = tileMono.gameObject;
                    
                    count += 1;
                    basicUnit.PlayableUnitID = count;

                    Bus<SetUpUnitHealthBar>.Raise(new SetUpUnitHealthBar(
                        basicUnit.PlayableUnitID,
                        1, 1,
                        basicUnit.UnitImage
                    ));
                    
                    basicUnit.SetObject(GaugeManager, endTurnBtn);

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