using System;
using System.Collections;
using UnityEngine;
using Code.Core.Interfaces;
using UnitSystem;


namespace Code.UnitSystem
{
    public class UnitMovement : MonoBehaviour, IUnitComponent
    {
        private BasicUnit _owner;

        private float _moveSpeed => _owner.unitSO.moveSpeed;

        private bool _isMoveing = false;
        
        public void Initialize(Unit owner)
        {
            _owner = owner as BasicUnit;
            
            _owner.inputSO.OnClickMoveEvent += Move;
        }

        /// <summary>
        /// 플레이어가 움직이는 코드
        /// </summary>
        private void Move()
        {
            if (!_owner.IsPlayerUnit)
                return;
            
            if (_isMoveing)
                return;
            
            _isMoveing = true;

            IMapTile tile = _owner.inputSO.GetSelectedTile();
            GameObject tileTrm = _owner.inputSO.GetWorldPosition();

            StartCoroutine(MoveStart(tile, tileTrm));
        }

        private IEnumerator MoveStart(IMapTile tileInfo,GameObject tile)
        {
            Debug.Log(tile.transform.position);
            Debug.Log(tileInfo.IsWalkable);
            
            if (tileInfo.IsWalkable)
            {
                while (Vector3.Distance(_owner.transform.position, tile.transform.position) > 0.01f)
                {
                    _owner.transform.position = Vector3.MoveTowards(
                        _owner.transform.position,
                        tile.transform.position,
                        _moveSpeed * Time.deltaTime
                    );

                    yield return null;
                }
            }
            _isMoveing = false;
        }
    }
}