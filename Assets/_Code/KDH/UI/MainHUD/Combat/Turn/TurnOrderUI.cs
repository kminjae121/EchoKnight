using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Managers;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.UI
{
    public class TurnOrderUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TurnManager turnManager;
        
        [Header("Settings")]
        [SerializeField] private int showTurnOrderCount = 5; 
        
        [Header("UI Pools")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private PoolingItemSO unitSlotPoolingSO;
        [SerializeField] private PoolingItemSO roundSlotPoolingSO;
        
        private PoolManagerMono _poolManager;
        
        private List<TurnOrderUnitSlotUI> _activeUnitSlots = new List<TurnOrderUnitSlotUI>();
        private List<TurnOrderRoundSlotUI> _activeRoundSlots = new List<TurnOrderRoundSlotUI>();

        private bool _isGhostComponent = false;

        private void Awake()
        {
            if (unitSlotPoolingSO == null || roundSlotPoolingSO == null)
            {
                Debug.LogWarning($"[TurnOrderUI] 풀링 SO가 할당되지 않은 유령 컴포넌트가 감지되어 이벤트를 차단합니다. 대상: [{gameObject.name}]");
                _isGhostComponent = true;
                return;
            }

            _poolManager = UnityEngine.Object.FindFirstObjectByType<PoolManagerMono>();
        }

        private void OnEnable()
        {
            if (_isGhostComponent) return;
            Bus<TurnOrderUpdateEvent>.Subscribe(HandleTurnOrderUpdate);
        }

        private void OnDisable()
        {
            if (_isGhostComponent) return;
            Bus<TurnOrderUpdateEvent>.Unsubscribe(HandleTurnOrderUpdate);
        }

        private void HandleTurnOrderUpdate(TurnOrderUpdateEvent evt)
        {
            if (turnManager == null || _poolManager == null) return;

            var units = turnManager.GetTimelineUnits(showTurnOrderCount);
            
            ClearAllSlots();

            for (int i = 0; i < showTurnOrderCount; ++i)
            {
                if (i < units.Count)
                {
                    var turnable = units[i];
                    
                    if (turnable is RoundTracker roundTracker)
                    {
                        var roundSlot = _poolManager.Pop<TurnOrderRoundSlotUI>(roundSlotPoolingSO);
                        if (roundSlot != null)
                        {
                            roundSlot.transform.SetParent(slotContainer != null ? slotContainer : transform);
                            roundSlot.transform.SetSiblingIndex(i);
                            roundSlot.transform.localScale = Vector3.one;
                            roundSlot.Setup(roundTracker);
                            _activeRoundSlots.Add(roundSlot);
                        }
                    }
                    else
                    {
                        var unitSlot = _poolManager.Pop<TurnOrderUnitSlotUI>(unitSlotPoolingSO);
                        if (unitSlot != null)
                        {
                            unitSlot.transform.SetParent(slotContainer != null ? slotContainer : transform);
                            unitSlot.transform.SetSiblingIndex(i);
                            unitSlot.transform.localScale = Vector3.one;
                            unitSlot.Setup(turnable);
                            _activeUnitSlots.Add(unitSlot);
                        }
                    }
                }
            }
        }

        private void ClearAllSlots()
        {
            foreach (var slot in _activeUnitSlots)
            {
                if (slot != null)
                {
                    slot.ReturnToPool();
                }
            }
            _activeUnitSlots.Clear();
                
            foreach (var slot in _activeRoundSlots)
            {
                if (slot != null)
                {
                    slot.ReturnToPool();
                }
            }
            _activeRoundSlots.Clear();
        }
    }
}