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
    public class UnitMovement : RangeComponent, IUnitComponent
    {
        private BasicUnit _unit;

        public UnityEvent moveEvent;

        private SetUnitCamera unitCam;
        
        private float _moveSpeed => _unit.unitSO.moveSpeed;
        
        [SerializeField] private UnitAnimation animationCompo;

        [SerializeField] private UnitRotation rotationCompo; 
        
        public void Initialize(Unit owner)
        {
            _unit = owner as BasicUnit;
            
            _unit.inputSO.OnClickMoveEvent += Move;
            
            Bus<UnitMoveEvent>.Subscribe(CheckCanMoveTile);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();
        }   


        /// <summary>
        /// 움직이게 해주는 코드
        /// </summary>
        /// <param name="evt"></param>
        public void CheckCanMoveTile(UnitMoveEvent evt)
        {
            if (evt.isMove)
            {
                if (_unit.isMyTurn)
                {
                    unitCam.SetThisUnit();
                    moveEvent?.Invoke();
                    FindObjectInRange(); 
                }
            }
            else
            {
                ResetTile();
                EndAct();
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

            if(tile == null)
                ResetTile();
            
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
            }
            animationCompo.PlaySelectAnimation("IDLE");   
            _isAct = false;
        }
        
    }
}