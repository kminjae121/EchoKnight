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
        
        private float _moveSpeed => _unit.unitSO.moveSpeed;
        
        [SerializeField] private UnitAnimation animationCompo;

        [SerializeField] private UnitRotation rotationCompo;

        private GameObject _currentMapTile = null;

        private List<GameObject> _movingtiles =  new List<GameObject>();

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
                    FindObjectInRange(); 
                    unitCam.SetThisUnit();
                }
            }
            else
            {
                ResetTile();
                EndAct();
            }
        }

        private void CheckTilesCanMoving()
        {
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

        /// <summary>
        /// 플레이어가 움직이는 코드
        /// </summary>
        private void Move()
        {
            if (!_unit.isMyTurn)
                return;
            
            if (!_isAct)
                return;
            
            CheckTilesCanMoving();

            IMapTile tile = _unit.inputSO.GetSelectedTile();
            GameObject tileTrm = _unit.inputSO.GetWorldPosition();

            if (!_movingtiles.Contains(tileTrm))
            {
                ResetTile();
                return;
            }

            if (tile == null)
            {
                ResetTile();
                return;
            }
            else
            {
                StartCoroutine(MoveStart(tile, tileTrm));
                ResetTile();
            }
            
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
                    _unit.RemoveCost(0.1f);
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
            
            animationCompo.PlaySelectAnimation("IDLE");   
            _isAct = false;
        }
        
    }
}