using System.Collections.Generic;
using Code.UnitSystem;
using UnityEngine;

namespace Code.UI
{
    public class UnitListUI : MonoBehaviour
    {
        [SerializeField] private UnitStorageSO unitStorage;
        [SerializeField] private UnitSlotUI slotPrefab;
        [SerializeField] private Transform contentTrm;

        private readonly List<UnitSlotUI> slotList = new();

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            foreach (var slot in slotList)
                Destroy(slot.gameObject);
            
            slotList.Clear();
            
            foreach (var unit in unitStorage.units)
            {
                var slot = Instantiate(slotPrefab, contentTrm);
                slot.SetUnit(unit);
                slotList.Add(slot);
            }
        }
    }
}