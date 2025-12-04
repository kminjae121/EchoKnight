using System;
using System.Collections;
using UnityEngine;
using System.Collections;
using Code.Core.Interfaces;
using UnityEngine.Tilemaps;

namespace UnitSystem
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
            if (!_owner.isPlayerUnit)
                return;
            
            if (_isMoveing)
                return;
            
            _isMoveing = true;

            IMapTile tile = _owner.inputSO.GetSelectedTile();
            //Vector3 moveingTileTrm = _owner.inputSO.GetWorldPosition();

            StartCoroutine(MoveStart(tile));
        }

        private IEnumerator MoveStart(IMapTile tileInfo)
        {
            if (tileInfo.IsWalkable)
            {
                Vector2Int targetPos = tileInfo.GridPosition;
            
                while (Vector2.Distance(_owner.transform.position,targetPos) > 0.01f)
                {
                    _owner.transform.position = Vector2.MoveTowards(_owner.transform.position, targetPos, _moveSpeed * Time.deltaTime);
                    yield return null;
                }   
            }
            _isMoveing = false;
        }
    }
}