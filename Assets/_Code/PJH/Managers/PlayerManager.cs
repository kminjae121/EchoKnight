using System.Collections.Generic;
using Code.Core;
using Code.Items;
using UnityEngine;

namespace Code.Managers
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        public int Gold { get; private set; }
        public List<EquipmentItemSO> equipmentInventory;

        public void AddGold(int value)
            => Gold += value;
    }
}