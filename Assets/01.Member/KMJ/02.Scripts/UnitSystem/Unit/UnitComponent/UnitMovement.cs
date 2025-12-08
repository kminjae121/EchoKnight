using System;
using System.Collections;
using Code.Core.Events.Bus;
using UnityEngine;
using Code.Core.Interfaces;
using UnitSystem;


namespace Code.UnitSystem
{
    public class UnitMovement : MonoBehaviour, IUnitComponent
    {
        private BasicUnit _unit;

        private float _moveSpeed => _unit.unitSO.moveSpeed;

        private bool _isMove = false;
        
        public void Initialize(Unit owner)
        {
            _unit = owner as BasicUnit;
            
            _unit.inputSO.OnClickMoveEvent += Move;
            
            Bus<UnitMoveEvent>.Subscribe(HandleMoveEvent);
        }

        private void HandleMoveEvent(UnitMoveEvent obj)
        {
            _isMove = obj.isMove;
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

        private IEnumerator MoveStart(IMapTile tileInfo,GameObject tile)
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
            }
            
            _unit.TurnEnd();
            _isMove = false;
        }
    }
}