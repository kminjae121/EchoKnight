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

        private void Awake()
        {
            atkBtn.onClick.AddListener(HandleAttack);
            moveBtn.onClick.AddListener(HandleMove);
        }

        private void HandleMove()
        {
            Bus<UnitMoveEvent>.Raise(new UnitMoveEvent(true));
        }

        private void HandleAttack()
        {
            Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(true));
        }
        
    }
}