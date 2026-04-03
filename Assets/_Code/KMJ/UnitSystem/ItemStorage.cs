using System.Collections.Generic;
using _Code.KMJ.UnitSystem;
using _Code.Passive;
using _Code.UnitSystem;
using Code.Core;
using Code.Items;
using UnityEngine;

namespace _Code.Item
{
    public class ItemStorage : MonoSingleton<ItemStorage>
    {
        private Dictionary<UnitType, List<EquipmentItemSO>> items 
            = new Dictionary<UnitType, List<EquipmentItemSO>>();

        public void SetItem(UnitType unitType, EquipmentItemSO itemSO)
        {
            if (items.TryGetValue(unitType, out var itemList))
            {
                if (!itemList.Contains(itemSO))
                {
                    itemList.Add(itemSO);
                    InGameStatCompo.Instance.SetStat(itemSO.StatInfo, itemSO.StatValue, unitType);
                    
                    if (itemSO.PassiveSO != null)
                        PassiveStorage.Instance.SetPassive(unitType, itemSO.PassiveSO);
                    
                }
            }
            else
            {
                var newList = new List<EquipmentItemSO>
                {
                    itemSO
                };
                items.Add(unitType, newList);

                InGameStatCompo.Instance.SetStat(itemSO.StatInfo, itemSO.StatValue, unitType);
                
                if (itemSO.PassiveSO != null)
                    PassiveStorage.Instance.SetPassive(unitType, itemSO.PassiveSO);
            }
        }

        public void RemoveItem(UnitType unitType, EquipmentItemSO itemSO)
        {
            if (items.TryGetValue(unitType, out var itemList))
            {
                if (itemList.Contains(itemSO))
                {
                    itemList.Remove(itemSO);
                    InGameStatCompo.Instance.SetStat(itemSO.StatInfo, -itemSO.StatValue, unitType);
                    
                    if (itemSO.PassiveSO != null)
                        PassiveStorage.Instance.SetPassive(unitType, itemSO.PassiveSO);
                    Debug.Log("와우");
                }
            }
        }
    }
}