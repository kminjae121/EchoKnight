using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using UnityEngine;
using Code.Core.Interfaces;
using UnitSystem;
using UnityEngine.Events;


namespace Code.UnitSystem
{
    public class UnitMovement : MonoBehaviour, IUnitComponent
    {
        private BasicUnit _unit;
        
        [SerializeField] private Vector3 _verticalCheckBoxSize;
        [SerializeField] private Vector3 _horizontalCheckBoxSize;

        [SerializeField] private LayerMask _whatIsGround;

        private Collider[] verticalCollider;
        private Collider[] horizontalCollider;

        public UnityEvent moveEvent;

        private SetUnitCamera unitCam;
        
        private float _moveSpeed => _unit.unitSO.moveSpeed;

        private bool _isMove = false;
        
        [SerializeField] private UnitAnimation animationCompo;

        [SerializeField] private UnitRotation rotationCompo; 
        
        public void Initialize(Unit owner)
        {
            _unit = owner as BasicUnit;
            
            _unit.inputSO.OnClickMoveEvent += Move;
            
            Bus<UnitMoveEvent>.Subscribe(CheckCanMoveTile);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();
        }   


        public void CheckCanMoveTile(UnitMoveEvent evt)
        {
            if (evt.isMove)
            {
                if (_unit.isMyTurn)
                {
                    unitCam.SetThisUnit();
                    moveEvent?.Invoke();
                    verticalCollider =Physics.OverlapBox(transform.position, _verticalCheckBoxSize, Quaternion.identity, _whatIsGround);
                    horizontalCollider = Physics.OverlapBox(transform.position, _horizontalCheckBoxSize, Quaternion.identity, _whatIsGround);
            
                    verticalCollider.ToList().ForEach(obj =>
                    {
                        IMapTile tile = obj.GetComponent<IMapTile>();

                        if (!tile.HasObstacle && !tile.HasEnemy)
                        {
                            tile.SetWalkable(true);
                        }
                    });
            
                    horizontalCollider.ToList().ForEach(obj =>
                    {
                        IMapTile tile = obj.GetComponent<IMapTile>();

                        if (!tile.HasObstacle && !tile.HasEnemy)
                        {
                            tile.SetWalkable(true);
                        }
                    });

                    _isMove = true;   
                }
            }
            else
            {
                ResetTile();
                _isMove = false;
            }
            
        }

        public void ResetTile()
        {
            if (horizontalCollider == null && verticalCollider == null)
                return;

            horizontalCollider.ToList().ForEach(obj =>
            {
                IMapTile tile = obj.GetComponent<IMapTile>();

                if (!tile.HasObstacle)
                {
                    tile.SetWalkable(false);
                }
            });
            
            verticalCollider.ToList().ForEach(obj =>
            {
                IMapTile tile = obj.GetComponent<IMapTile>();

                if (!tile.HasObstacle)
                {
                    tile.SetWalkable(false);
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
            
            if (!_isMove)
                return;

            IMapTile tile = _unit.inputSO.GetSelectedTile();
            GameObject tileTrm = _unit.inputSO.GetWorldPosition();

            if(tile == null)
                ResetTile();
            
            else
            {
                StartCoroutine(MoveStart(tile, tileTrm));
                ResetTile();
            }
            
            unitCam.EndThisUnit();
        }

        private IEnumerator MoveStart(IMapTile tileInfo, GameObject tile)
        {
            if (tile == null) 
                yield break;
            
            if(tileInfo == null)
                yield break;
            
            rotationCompo.SetDir(tile.transform.position);
            if (tileInfo.IsWalkable)
            {
                animationCompo.PlaySelectAnimation("MOVE");   
                while (Vector3.Distance(_unit.transform.position, tile.transform.position) > 0.01f)
                {
                    _unit.transform.position = Vector3.MoveTowards(
                        _unit.transform.position,
                        tile.transform.position,
                        _moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }
                //Bus<UnitMoveEvent>.Raise(new UnitMoveEvent(false));
            }
            animationCompo.PlaySelectAnimation("IDLE");   
            _isMove = false;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _verticalCheckBoxSize);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, _horizontalCheckBoxSize);
        }
    }
}