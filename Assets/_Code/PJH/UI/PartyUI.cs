using System.Collections.Generic;
using Code.Core.Events.Bus;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.UI
{
    public class PartyUI : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private PoolingItemSO partySlotPoolingSO;
        
        [Header("Containers")]
        [SerializeField] private Transform slotContainer;

        [Inject] private PoolManagerMono _poolManager;
        
        private List<PartyCharacterSlotUI> _activeSlots = new();

        private void Awake()
        {
            if (_poolManager == null)
                _poolManager = FindFirstObjectByType<PoolManagerMono>();
        }

        private void OnEnable()
        {
            Bus<PartyCharacterSelectEvent>.Subscribe(HandlePartySelectEvent);
        }

        private void OnDisable()
        {
            Bus<PartyCharacterSelectEvent>.Unsubscribe(HandlePartySelectEvent);
        }
        
        private void HandlePartySelectEvent(PartyCharacterSelectEvent evt)
        {
            RefreshPartyList();
        }

        private void RefreshPartyList()
        {
            _activeSlots.Clear();
        }
    }
}