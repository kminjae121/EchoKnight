using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.EntityComponent;
using Code.UnitSystem;
using EnemySystem;
using EntityComponent;
using TMPro;
using UnitSystem;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace _Code.KMJ.UnitSystem.Unit.UnitComponent
{
    public class UnitBehavaveCompo : RangeComponent
    {
        private BasicUnit _unit;
        
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

        private IMapTile nextTile = null;
        
        protected override void Start()
        {
            base.Start();
            
            _unit = _owner as BasicUnit;
            
            _unit.inputSO.OnClickMoveEvent += Move;

            _moveSpeed = _unit.unitStatCompo.GetStat<float>(StatInfo.MoveSpeed);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();
            
            navMeshAgent.updatePosition = true;
            
            navMeshAgent.updateRotation = true;
            
            navMeshAgent.speed = _moveSpeed;
            
            navMeshAgent.acceleration = 999f;
            
            navMeshAgent.stoppingDistance = 0.05f;
            
            navMeshAgent.enabled = false;
        }

        protected override void OnDestroy()
        {
            _unit.inputSO.OnClickMoveEvent -= Move;
            
            base.OnDestroy();
        }

        private void OnDisable()
        {
            if (nextTile != null)
            {
                nextTile.SetEnemy(false);
            }
        }


        private void Update()
        {
            if (_unit.isMyTurn && _isAct && !isMoving)
            {
                CheckTilesCanMoving();
                GameObject tileTrm = _unit.inputSO.GetWorldPosition();
            
                if (_movingtiles.Contains(tileTrm))
                {
                    if (nextTile != null)
                    {
                        nextTile.SetEnemy(false);
                    }
                    visualPrefabs.transform.rotation = _unit.transform.rotation;
                    visualPrefabs.SetActive(true);
                    visualPrefabs.transform.rotation = _unit.transform.rotation; 
                    visualPrefabs.transform.position = tileTrm.transform.position;
                    nextTile = tileTrm.GetComponent<IMapTile>();
                    nextTile.SetEnemy(true);
                }
                else
                {
                    if (nextTile != null)
                    {
                        nextTile.SetEnemy(false);
                    }
                    visualPrefabs.SetActive(false);
                }
            }
            else if (_isAct == false)
            {
                if (nextTile != null)
                {
                    nextTile.SetEnemy(false);
                }
                visualPrefabs.SetActive(false);
            }
        }
        
        private void CheckTilesCanMoving()
        {
            _movingtiles.Clear();
            
            _horizontalCollider.ToList().ForEach(tile =>
            {
                if (tile.TryGetComponent(out IMapTile tiled))
                {
                    if (!tiled.HasObstacle)
                    {
                        _movingtiles.Add(tile.gameObject);
                    }
                }
            });
            
            _verticalCollider.ToList().ForEach(tile =>
            {
                if (tile.TryGetComponent(out IMapTile tiled))
                {
                    if (!tiled.HasObstacle)
                    {
                        _movingtiles.Add(tile.gameObject);
                    }
                }
            });
            
        }
        
        
        
        
        
        private void Move()
        {
            if (!_unit.isMyTurn)
                return;
            if (!_isAct)
                return;

            if (isMoving)
                return;
            
            if (_unit.GetCurrentCost() <= 0)
            {
                Bus<WarningUIEvent>.Raise(new WarningUIEvent("AP가 부족합니다"));
                ResetTile();
                EndAct();
                return;
            }
            
            IMapTile tile = _unit.inputSO.GetSelectedTile();
            GameObject tileTrm = _unit.inputSO.GetWorldPosition();

            if (!_movingtiles.Contains(tileTrm))
            {
                visualPrefabs.SetActive(false);
                return;
            }
            

            if (tile == null)
            {
                visualPrefabs.SetActive(false);
                return;
            }
            else
            {
                visualPrefabs.SetActive(false);
                StartCoroutine(Move(tile, tileTrm));
            }
            
            visualPrefabs.SetActive(false);
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
            
            yield return new WaitForSeconds(0.2f);
            
            MoveStart(tile);

            _unit.RemoveCost(15);

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

        private void MoveStart(GameObject tile)
        {
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unit.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
            visualPrefabs.SetActive(false);
            _isAct = false;
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
            _isAct = true;
            
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
    }
}