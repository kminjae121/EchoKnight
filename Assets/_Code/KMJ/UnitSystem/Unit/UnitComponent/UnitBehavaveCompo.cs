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
        [SerializeField] private GameObject _visualPrefabs;

        [SerializeField] private NavMeshAgent navMeshAgent;
        
        private bool isMoving = false;
        private float _moveSpeed;
        public GameObject _currentMapTile { get; set; }= null;
        
        private List<GameObject> _movingtiles =  new List<GameObject>();

        private Transform nextTile;
        
        protected override void Start()
        {
            base.Start();
            
            _unit = _owner as BasicUnit;
            
            _unit.inputSO.OnClickMoveEvent += Move;

            _moveSpeed = _unit.unitStatCompo.GetStat<float>(StatInfo.MoveSpeed);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();

            nextTile = transform;
            navMeshAgent.enabled = false;
        }

        protected override void OnDestroy()
        {
            _unit.inputSO.OnClickMoveEvent -= Move;
            base.OnDestroy();
        }


        private void Update()
        {
            if (_unit.isMyTurn && _isAct && !isMoving)
            {
                CheckTilesCanMoving();
                GameObject tileTrm = _unit.inputSO.GetWorldPosition();
            
                if (_movingtiles.Contains(tileTrm))
                {
                    rotationCompo.SetDir(_visualPrefabs.transform.position);
                    _visualPrefabs.transform.rotation = _unit.transform.rotation;
                    _visualPrefabs.SetActive(true);
                    _visualPrefabs.transform.rotation = _unit.transform.rotation; 
                    _visualPrefabs.transform.position = tileTrm.transform.position;
                }
                else
                {
                    _visualPrefabs.SetActive(false);
                }
            }
            else if (_isAct == false)
            {
                _visualPrefabs.SetActive(false);
            }


            if (isMoving && _unit.isMyTurn)
            {
                if (navMeshAgent.enabled == true)
                {
                    navMeshAgent.SetDestination(nextTile.transform.position);
                }
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
                _visualPrefabs.SetActive(false);
                return;
            }
            

            if (tile == null)
            {
                _visualPrefabs.SetActive(false);
                return;
            }
            else
            {
                _visualPrefabs.SetActive(false);
                StartCoroutine(MoveStart(tile, tileTrm));
            }
            
            _visualPrefabs.SetActive(false);
        }
        
        
        
        private IEnumerator MoveStart(IMapTile tileInfo, GameObject tile)
        {
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unit.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
            navMeshAgent.enabled = true;
            navMeshAgent.acceleration = 999f; 
            _visualPrefabs.SetActive(false);
            _isAct = false;
            isMoving = true;
            
            
            if (tile == null) 
                yield break;
            
            if(tileInfo == null)
                yield break;

            if (!tileInfo.IsWalkable)
                yield break;

            navMeshAgent.speed = _moveSpeed;
            
            animationCompo.PlaySelectAnimation("MOVE"); 
            if (_currentMapTile != null)
            {
                IMapTile tileInfos = _currentMapTile.GetComponent<IMapTile>();
                tileInfos.SetObstacle(false);
                tileInfos.SetWalkable(true);
            }
            
            navMeshAgent.ResetPath();    
            navMeshAgent.isStopped = false;
            
            
            nextTile.transform.position = tile.transform.position;

            
            yield return new WaitForSeconds(0.3f);
            
            rotationCompo.SetDir(tile.transform.position);
            _unit.RemoveCost(15);
            
            while (Vector3.Distance(tile.transform.position, _unit.transform.position) >= 2f)
            {
                Debug.Log(Vector3.Distance(tile.transform.position, _unit.transform.position));
                yield return null;
            }
            
            navMeshAgent.ResetPath();
            navMeshAgent.speed = 0;
            navMeshAgent.enabled = false;
            

            
            while (Vector2.Distance(new Vector2(tile.transform.position.x, tile.transform.position.z),
                       new Vector2(_unit.transform.position.x, _unit.transform.position.z)) >= 0.1f)
            {
                Vector3 target = tile.transform.position;
                target.y = _unit.transform.position.y;

                _unit.transform.position = Vector3.MoveTowards(
                    _unit.transform.position,
                    target,
                    _moveSpeed * Time.deltaTime
                );

                yield return null;
            }
            
            
            isMoving = false;
            _isAct = true;
            
            _currentMapTile = tile;
            tile.transform.TryGetComponent(out IMapTile EndMapTile);

            EndMapTile.SetObstacle(true);
            _visualPrefabs.SetActive(true);
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            _owner.transform.position = tile.transform.position;
            animationCompo.PlaySelectAnimation("IDLE");   
        }
    }
}