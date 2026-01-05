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
        }

        private void HandleMove()
        {
            if (isMoveing)
            {
                Bus<UnitMoveEvent>.Raise(new UnitMoveEvent(true));
                isMoveing = false;
            }
            else
            {
                Bus<UnitMoveEvent>.Raise(new UnitMoveEvent(false));
                isMoveing = true;
            }
        }

        public void SetMovingTrue()
        {
            isMoveing = true;
        }

        public void SetAttackingTrue()
        {
            isAttacking = true;
        }
        

        private void HandleAttack()
        {
            if (isAttacking)
            {
                Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(true));
                isAttacking = false;
            }
            else
            {
                Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(false));
                isAttacking = true;
            }
        }
        
    }
}