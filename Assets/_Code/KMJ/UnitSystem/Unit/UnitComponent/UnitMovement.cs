using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using UnityEngine;
using Code.Core.Interfaces;
using UnitSystem;
using UnityEngine.Events;
using Debug = System.Diagnostics.Debug;


namespace Code.UnitSystem
{
    public class UnitMovement : RangeComponent
    {
        private BasicUnit _unit;

        public UnityEvent moveEvent;

        private SetUnitCamera unitCam;
        
        private float _moveSpeed => _unit.unitSO.MoveSpeed;
        
        [SerializeField] private UnitAnimation animationCompo;

        [SerializeField] private UnitRotation rotationCompo;

        public GameObject _currentMapTile { get; set; }= null;

        private List<GameObject> _movingtiles =  new List<GameObject>();

        [SerializeField] private GameObject _visualPrefabs;

        protected override void Start()
        {
            base.Start();
            _unit = _owner as BasicUnit;
            
            _unit.inputSO.OnClickMoveEvent += Move;
            
            Bus<UnitMoveEvent>.Subscribe(CheckCanMoveTile);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _unit.inputSO.OnClickMoveEvent -= Move;
        }


        /// <summary>
        /// 움직이게 해주는 코드
        /// </summary>
        /// <param name="evt"></param>
        public void CheckCanMoveTile(UnitMoveEvent evt)
        {
            moveEvent?.Invoke();
            if (evt.isMove)
            {
                if (_unit.isMyTurn)
                {
                    if (_unit.GetCurrentCost() <= 0)
                    {
                        Bus<WarningUIEvent>.Raise(new WarningUIEvent("AP가 부족합니다"));
                        ResetTile();
                        EndAct();
                        return;
                    }
                   
                    FindObjectInRange(); 
                    unitCam.SetThisUnit();
                }
                else
                {
                    ResetTile();
                    _visualPrefabs.SetActive(false);
                    EndAct();
                }
            }
            else
            {
                
                ResetTile();
                _visualPrefabs.SetActive(false);
                EndAct();
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

        private void Update()
        {
            if (_unit.isMyTurn && _isAct)
            {
                CheckTilesCanMoving();
                GameObject tileTrm = _unit.inputSO.GetWorldPosition();
                
                if (_movingtiles.Contains(tileTrm))
                {
                    rotationCompo.SetDir(_visualPrefabs.transform.position);
                    _visualPrefabs.SetActive(true);
                    _visualPrefabs.transform.position = tileTrm.transform.position;
                }
            }
        }

        /// <summary>
        /// 플레이어가 움직이는 코드
        /// </summary>
        private void Move()
        {
            if (!_unit.isMyTurn)
                return;
            
            if (!_isAct)
                return;
            
            IMapTile tile = _unit.inputSO.GetSelectedTile();
            GameObject tileTrm = _unit.inputSO.GetWorldPosition();

            if (!_movingtiles.Contains(tileTrm))
            {
                ResetTile();
                _visualPrefabs.SetActive(false);
                return;
            }

            if (tile == null)
            {
                ResetTile();
                _visualPrefabs.SetActive(false);
                return;
            }
            else
            {
                _visualPrefabs.SetActive(false);
                StartCoroutine(MoveStart(tile, tileTrm));
                ResetTile();
            }
            
            _visualPrefabs.SetActive(false);
            unitCam.EndThisUnit();
        }

        /// <summary>
        /// 움직이는 코드
        /// </summary>
        /// <param name="tileInfo">움직일 타일의 정보컴포넌트</param>
        /// <param name="tile">움직일 타일의 Transform </param>
        /// <returns></returns>
        private IEnumerator MoveStart(IMapTile tileInfo, GameObject tile)
        {
            if (tile == null) 
                yield break;
            
            if(tileInfo == null)
                yield break;
            
            if(_currentMapTile != null)
                _currentMapTile.GetComponent<IMapTile>().SetObstacle(false);
            
            rotationCompo.SetDir(tile.transform.position);
            if (tileInfo.IsWalkable)
            {
                animationCompo.PlaySelectAnimation("MOVE");   
                while (Vector3.Distance(_unit.transform.position, tile.transform.position) > 0.01f)
                {
                    _unit.RemoveCost(0.03f);
                    _unit.transform.position = Vector3.MoveTowards(
                        _unit.transform.position,
                        tile.transform.position,
                        _moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }
            }

            _currentMapTile = tile;

            tile.transform.TryGetComponent(out IMapTile EndMapTile);

            EndMapTile.SetObstacle(true);
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            
            animationCompo.PlaySelectAnimation("IDLE");   
            _isAct = false;
        }
        
    }
}