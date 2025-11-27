using System;
using System.Collections;
using UnityEngine;
using System.Collections;
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
            if (!_owner.isSelect)
                return;
            
            if (_isMoveing)
                return;
            
            _isMoveing = true;
            
            Vector3 moveingTileTrm = _owner.inputSO.GetWorldPosition();

            StartCoroutine(MoveStart(moveingTileTrm));
        }

        private IEnumerator MoveStart(Vector3 targetTile)
        {
            Vector3 targetPos = targetTile;
            
            while (Vector3.Distance(_owner.transform.position, targetPos) > 0.01f)
            {
                _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, targetPos, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            _isMoveing = false;
        }
    }
}