using System.Collections;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Map;
using UnityEngine;
using UnityEngine.AI;

namespace Code.UnitSystem
{
    public class UnitBehaviorCompo : RangeComponent
    {
        [SerializeField] private UnitAnimation animationCompo;
        [SerializeField] private UnitRotation rotationCompo;
        [SerializeField] private UnitAnimationTrigger triggerCompo;
        [field: SerializeField] public GameObject VisualPrefabs { get; set; }
        [SerializeField] private NavMeshAgent navMeshAgent;

        public IMapTile CurrentMapTile { get; set; }
        public UnitManageRangeCompo UnitRangeCompo { get; private set; }

        private readonly List<IMapTile> _movingTiles = new();

        private CharacterUnit _unit;
        private bool _isMoving;
        private float _moveSpeed;
        private UnitCostComponent _unitCostComponentCompo;
        private IMapTile _nextTile;

        protected override void Start()
        {
            base.Start();

            _unit = _owner as CharacterUnit;
            _unitCostComponentCompo = _unit.GetUnitCompo<UnitCostComponent>();
            UnitRangeCompo = _unit.GetUnitCompo<UnitManageRangeCompo>();

            _moveSpeed = 9;

            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = true;
            navMeshAgent.speed = _moveSpeed;
            navMeshAgent.acceleration = 999f;
            navMeshAgent.stoppingDistance = 0.05f;
            navMeshAgent.enabled = false;

            _unit.InputSO.OnClickMoveEvent += Move;
            _unit.InputSO.OnCancelEvent += HandleResetTile;

            Bus<UnitSetMoveEvent>.Subscribe(StartWalk);

            _isMoving = false;
        }

        private void OnDisable()
        {
            _nextTile?.SetEnemy(false);

            _unit.InputSO.OnCancelEvent -= HandleResetTile;
            _unit.InputSO.OnClickMoveEvent -= Move;

            Bus<UnitSetMoveEvent>.Unsubscribe(StartWalk);
        }

        private void EndTargeting()
        {
            _nextTile?.SetEnemy(false);
            VisualPrefabs.SetActive(false);
        }

        private void SetTargetEnemy(IMapTile tile)
        {
            _nextTile?.SetEnemy(false);

            VisualPrefabs.SetActive(true);
            VisualPrefabs.transform.rotation = _unit.transform.rotation;
            VisualPrefabs.transform.position = tile.WorldPos;

            _nextTile = tile;
            //_nextTile.SetEnemy(true);
        }

        private void CheckTilesCanMoving()
        {
            _movingTiles.Clear();

            foreach (var tile in _tilesInRange)
                if (!tile.HasObstacle && !tile.HasEnemy)
                    _movingTiles.Add(tile);
        }

        private void Update()
        {
            if (_unit.isMyTurn && IsActive && !_isMoving)
            {
                CheckTilesCanMoving();

                IMapTile tile = _unit.InputSO.GetSelectedTile();

                if (tile != null && _movingTiles.Contains(tile))
                    SetTargetEnemy(tile);
                else
                    EndTargeting();
            }
            else if (!IsActive)
                EndTargeting();
        }

        public void StartWalk(UnitSetMoveEvent evt)
        {
            if (_unit.isMyTurn && !evt.isStart)
                ResetTile();
            else if (_unit.isMyTurn && evt.isStart)
                ReCheckInRange();
        }


        private void HandleResetTile()
        {
            UnitRangeCompo.RemoveAllRange();
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
        }

        private void Move()
        {
            if (!_unit.isMyTurn)
                return;
            
            if (!IsActive)
                return;

            if (_isMoving)
                return;

            if (_unitCostComponentCompo.GetCurrentCost() <= 0)
            {
                Bus<WarningUIEvent>.Raise(new WarningUIEvent("AP가 부족합니다"));
                ResetTile();
                EndAct();
                return;
            }

            IMapTile tile = _unit.InputSO.GetSelectedTile();
            
            VisualPrefabs.SetActive(false);

            if (!_movingTiles.Contains(tile))
                return;
            
            StartCoroutine(Move(tile));
        }
        
        private void MoveStart(IMapTile tile)
        {
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unit.gameObject,
                true, new Vector3(0.1f, 0.1f, 0.1f)));
            
            GridMap.Instance.SetGridVisible(false);
            IsActive = false;
            _isMoving = true;

            rotationCompo.SetDir(tile.WorldPos);

            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.ResetPath();
            
            animationCompo.PlaySelectAnimation("MOVE");

            navMeshAgent.SetDestination(tile.WorldPos);
        }

        private void MoveEnd(IMapTile tile)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;

            GridMap.Instance.SetGridVisible(true);
            _isMoving = false;
            IsActive = true;

            CurrentMapTile = tile;
            tile.SetObstacle(true);

            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null,
                false, new Vector3(0.1f, 0.1f, 0.1f)));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            animationCompo.PlaySelectAnimation("IDLE");

            ResetTile();
            FindObjectInRange();
        }

        private IEnumerator Move(IMapTile tileInfo)
        {
            if (tileInfo == null)
                yield break;

            if (!tileInfo.IsWalkable)
                yield break;

            if (CurrentMapTile != null)
            {
                IMapTile tileInfos = CurrentMapTile;
                tileInfos.SetObstacle(false);
                tileInfos.SetWalkable(true);
            }

            MoveStart(tileInfo);

            _unitCostComponentCompo.RemoveCost(15);
 
            while (navMeshAgent.pathPending)
                yield return null;

            while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                if (navMeshAgent.velocity.sqrMagnitude < 0.0001f && !navMeshAgent.pathPending)
                    navMeshAgent.SetDestination(tileInfo.WorldPos);

                yield return null;
            }

            MoveEnd(tileInfo);
        }
    }
}