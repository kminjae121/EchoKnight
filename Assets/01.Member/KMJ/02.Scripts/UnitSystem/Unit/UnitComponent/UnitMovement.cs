using System;
using System.Collections;
using System.Linq;
using Code.Core.Events.Bus;
using UnityEngine;
using Code.Core.Interfaces;
using UnitSystem;


namespace Code.UnitSystem
{
    public class UnitMovement : MonoBehaviour, IUnitComponent
    {
        private BasicUnit _unit;
        
        [SerializeField] private Vector3 _verticalCheckBoxSize;
        [SerializeField] private Vector3 _horizontalCheckBoxSize;

        [SerializeField] private LayerMask _whatIsGround;
        
        private float _moveSpeed => _unit.unitSO.moveSpeed;

        private bool _isMove = false;
        
        public void Initialize(Unit owner)
        {
            _unit = owner as BasicUnit;
            
            _unit.inputSO.OnClickMoveEvent += Move;
            
            Bus<UnitMoveEvent>.Subscribe(CheckCanMoveTile);
        }   


        public void CheckCanMoveTile(UnitMoveEvent evt)
        {
            if (_unit.isMyTurn)
            {
                Collider[] collider =Physics.OverlapBox(transform.position, _verticalCheckBoxSize, Quaternion.identity, _whatIsGround);
                Collider[] collider2 = Physics.OverlapBox(transform.position, _horizontalCheckBoxSize, Quaternion.identity, _whatIsGround);
            
                collider.ToList().ForEach(obj =>
                {
                    IMapTile tile = obj.GetComponent<IMapTile>();

                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(true);
                        tile.SetEnemy(false);
                    }
                });
            
                collider2.ToList().ForEach(obj =>
                {
                    IMapTile tile = obj.GetComponent<IMapTile>();

                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(true);
                        tile.SetEnemy(false);
                    }
                });

                _isMove = true;   
            }
        }

        public void ResetTile()
        {
            
            Collider[] collider =Physics.OverlapBox(transform.position, _verticalCheckBoxSize, Quaternion.identity, _whatIsGround);
            Collider[] collider2 = Physics.OverlapBox(transform.position, _horizontalCheckBoxSize, Quaternion.identity, _whatIsGround);

            
            collider.ToList().ForEach(obj =>
            {
                IMapTile tile = obj.GetComponent<IMapTile>();

                if (tile.IsWalkable)
                {
                    tile.SetWalkable(false);
                }
            });
            
            collider2.ToList().ForEach(obj =>
            {
                IMapTile tile = obj.GetComponent<IMapTile>();

                if (tile.IsWalkable)
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

            StartCoroutine(MoveStart(tile, tileTrm));
        }

        private IEnumerator MoveStart(IMapTile tileInfo, GameObject tile)
        {
            Debug.Log(tile.transform.position);
            Debug.Log(tileInfo.IsWalkable);

            if (tileInfo.IsWalkable)
            {
                while (Vector3.Distance(_unit.transform.position, tile.transform.position) > 0.01f)
                {
                    _unit.transform.position = Vector3.MoveTowards(
                        _unit.transform.position,
                        tile.transform.position,
                        _moveSpeed * Time.deltaTime
                    );

                    yield return null;
                }

                Bus<UnitMoveEvent>.Raise(new UnitMoveEvent(false));
                _unit.TurnEnd();
            }

            ResetTile();
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