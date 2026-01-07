using System;
using Code.Core.Events.Bus;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UnitControl : MonoBehaviour
    {
        [SerializeField] private Button atkBtn;
        [SerializeField] private Button moveBtn;

        public bool isMoveing = true;
        public bool isAttacking = true;

        private void Awake()
        {
            atkBtn.onClick.AddListener(HandleAttack);
            moveBtn.onClick.AddListener(HandleMove);
            
            Bus<UnitMoveControlEvent>.Subscribe(SetMoving);
            Bus<UnitAttackControlEvent>.Subscribe(SetAttacking);
        }

        private void OnDestroy()
        {
            atkBtn.onClick.RemoveListener(HandleAttack);
            moveBtn.onClick.RemoveListener(HandleMove);
            
            Bus<UnitMoveControlEvent>.Unsubscribe(SetMoving);
            Bus<UnitAttackControlEvent>.Unsubscribe(SetAttacking);
        }

        private void HandleMove()
        {
            if (isMoveing)
            {
                Bus<UnitMoveEvent>.Raise(new UnitMoveEvent(true));
                isMoveing = false;
                isAttacking = true;
            }
            else
            {
                Bus<UnitMoveEvent>.Raise(new UnitMoveEvent(false));
                isMoveing = true;
            }
        }

        public void SetMoving(UnitMoveControlEvent evt)
        {
            isMoveing =  evt.isMoving;
        }

        public void SetAttacking(UnitAttackControlEvent evt)
        {
            isAttacking = evt.isAttacking;
        }
        

        private void HandleAttack()
        {
            if (isAttacking)
            {
                Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(true));
                isAttacking = false;
                isMoveing = true;
            }
            else
            {
                Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(false));
                isAttacking = true;
            }
        }
        
    }
}