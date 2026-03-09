using System.Collections;
using System.Collections.Generic;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;
using UnityEngine.AI;

namespace _Code.KMJ.UnitSystem.Unit.UnitComponent
{
    public class UnitBehavaveCompo : RangeComponent
    {
        private CharacterUnit _unit;
        
        private SetUnitCamera unitCam;
        [SerializeField] private UnitAnimation animationCompo;

        [SerializeField] private UnitRotation rotationCompo;
        [SerializeField] private UnitAnimationTrigger triggerCompo;
        [field:SerializeField] public GameObject visualPrefabs { get; set; }

        [SerializeField] private NavMeshAgent navMeshAgent;
        
        private bool isMoving = false;
        private float _moveSpeed;
        public GameObject _currentMapTile { get; set; }= null;
        
        private List<GameObject> _movingtiles =  new List<GameObject>();
        public UnitManageRangeCompo unitRangeCompo { get; private set; }
        private UnitCostComponent _unitCostComponentCompo;

        private IMapTile nextTile = null;

        
        protected override void Start()
        {
            base.Start();
            
            _unit = _owner as CharacterUnit;
            
            _unit.InputSO.OnClickMoveEvent += Move;

            _moveSpeed = _unit.UnitStatCompo.GetStat<float>(StatInfo.MoveSpeed);

            Bus<TopCamEvent>.Subscribe(HandleCamEvent);
            
            navMeshAgent.updatePosition = true;
            
            navMeshAgent.updateRotation = true;
            
            navMeshAgent.speed = _moveSpeed;
            
            navMeshAgent.acceleration = 999f;
            
            navMeshAgent.stoppingDistance = 0.05f;
            
            navMeshAgent.enabled = false;
            
            _unitCostComponentCompo = _unit.GetUnitCompo<UnitCostComponent>();
            
            Bus<UnitSetMoveEvent>.Subscribe(StartWalk);
            
            unitRangeCompo =  _unit.GetUnitCompo<UnitManageRangeCompo>();
            
            _unit.InputSO.OnCancelEvent += HandleResetTile;
        }

        private void HandleCamEvent(TopCamEvent obj)
        {
            unitCam = obj.cam.GetComponent<SetUnitCamera>();
        }

        protected override void OnDestroy()
        {
            _unit.InputSO.OnClickMoveEvent -= Move;
            
            base.OnDestroy();
        }

        private void OnDisable()
        {
            if (nextTile != null)
            {
                nextTile.SetEnemy(false);
            }
            _unit.InputSO.OnCancelEvent -= HandleResetTile;
            Bus<TopCamEvent>.Unsubscribe(HandleCamEvent);
            Bus<UnitSetMoveEvent>.Unsubscribe(StartWalk);
        }


        private void EndTargeting()
        {
            if (nextTile != null)
                nextTile.SetEnemy(false);

            visualPrefabs.SetActive(false);
        }

        private void SetTargetEnemy(GameObject tileTrm)
        {
            if (nextTile != null)
                nextTile.SetEnemy(false);
            
            visualPrefabs.transform.rotation = _unit.transform.rotation;
            visualPrefabs.SetActive(true);
            visualPrefabs.transform.rotation = _unit.transform.rotation; 
            visualPrefabs.transform.position = tileTrm.transform.position;
            nextTile = tileTrm.GetComponent<IMapTile>();
            nextTile.SetEnemy(true);
        }

        private void CheckTilesCanMoving()
        {
            _movingtiles.Clear();

            foreach (var tile in _horizontalCollider)
            {
                if (!tile.TryGetComponent(out IMapTile tiled))
                    continue;
                
                if (!tiled.HasObstacle)
                    _movingtiles.Add(tile.gameObject);
            }

            foreach (var tile in _verticalCollider)
            {
                if (!tile.TryGetComponent(out IMapTile tiled))
                    continue;
                
                if (!tiled.HasObstacle)
                    _movingtiles.Add(tile.gameObject);
            }
        }

        private void Update()
        {
            if (_unit.isMyTurn && IsActive && !isMoving)
            {
                CheckTilesCanMoving();
                
                GameObject tileTrm = _unit.InputSO.GetWorldPosition();
            
                if (_movingtiles.Contains(tileTrm))
                    SetTargetEnemy(tileTrm);
                else
                  EndTargeting();
            }
            else if (!IsActive)
                EndTargeting();
        }
        
        public void StartWalk(UnitSetMoveEvent evt)
        {
            if (_unit.isMyTurn  && evt.isStart == false)
            { 
                ResetTile();
            }
            else if (_unit.isMyTurn && evt.isStart == true)
            {
               ReCheckInRange();
            }
        }
        
        
        private void HandleResetTile()
        {
            unitRangeCompo.RemoveAllRange();
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
        }
        
        private void Move()
        {
            if (!_unit.isMyTurn)
                return;
            if (!IsActive)
                return;

            if (isMoving)
                return;
            
            if (_unitCostComponentCompo.GetCurrentCost() <= 0)
            {
                Bus<WarningUIEvent>.Raise(new WarningUIEvent("AP가 부족합니다"));
                ResetTile();
                EndAct();
                return;
            }
            
            IMapTile tile = _unit.InputSO.GetSelectedTile();
            GameObject tileTrm = _unit.InputSO.GetWorldPosition();

            if (!_movingtiles.Contains(tileTrm))
            {
                visualPrefabs.SetActive(false);
                return;
            }
            
            visualPrefabs.SetActive(false);
            
            StartCoroutine(Move(tile, tileTrm));
            
            visualPrefabs.SetActive(false);
        }
        
        
        
        private void MoveStart(GameObject tile)
        {
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unit.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
            
            visualPrefabs.SetActive(false);
            
            IsActive = false;
            isMoving = true;
            
            rotationCompo.SetDir(tile.transform.position);
            
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.ResetPath();

            rotationCompo.SetDir(tile.transform.position);
            animationCompo.PlaySelectAnimation("MOVE");
            
            navMeshAgent.SetDestination(tile.transform.position);
        }

        private void MoveEnd(GameObject tile)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;

            isMoving = false;
            IsActive = true;
            
            _currentMapTile = tile;
            tile.TryGetComponent(out IMapTile endTile);
            endTile.SetObstacle(true);

            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false, new Vector3(0.1f,0.1f,0.1f)));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            animationCompo.PlaySelectAnimation("IDLE");
            
            ResetTile();
            FindObjectInRange();
        }
        
        
        private IEnumerator Move(IMapTile tileInfo, GameObject tile)
        {
            if (tile == null) 
                yield break;
            
            if(tileInfo == null)
                yield break;

            if (!tileInfo.IsWalkable)
                yield break;
            
            if (_currentMapTile != null)
            {
                IMapTile tileInfos = _currentMapTile.GetComponent<IMapTile>();
                tileInfos.SetObstacle(false);
                tileInfos.SetWalkable(true);
            }
            
            MoveStart(tile);

            _unitCostComponentCompo.RemoveCost(15);

            while (navMeshAgent.pathPending) yield return null;

            while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                if (navMeshAgent.velocity.sqrMagnitude < 0.0001f && !navMeshAgent.pathPending)
                {
                    navMeshAgent.SetDestination(tile.transform.position);
                }
                yield return null;
            }
            
            MoveEnd(tile);
        }

    }
}