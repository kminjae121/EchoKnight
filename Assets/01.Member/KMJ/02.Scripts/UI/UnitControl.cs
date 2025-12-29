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

        private bool isMoveing = true;
        private bool isAttacking = true;

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