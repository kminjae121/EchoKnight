using System;
using Code.Core.Debugs;
using Code.Items;
using Code.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class StoreItemBtn : MonoBehaviour
    {
        [SerializeField] private Button itemButton;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private Image itemImage;
        
        [SerializeField] private TextMeshProUGUI goldTxt;
        
        private ItemSO item;

        private void Awake()
        {
            itemButton.onClick.AddListener(HandleItemButton);
        }

        private void OnDisable()
        {
            itemButton.onClick.RemoveListener(HandleItemButton);
        }

        public void SetItem(ItemSO newItem)
        {
            item = newItem;

            itemNameText.text = item.itemName;
            itemImage.sprite = item.itemIcon;
        }

        private void HandleItemButton()
        {
            if (item == null)
                return;
            
           
            switch (item)
            {
                case CurrencyItemSO currency:
                    HandleCurrency(currency);
                    break;
                case EquipmentItemSO equipment:
                    HandleEquipment(equipment);
                    break;
            }
            
            goldTxt.text = $"골드 : {PlayerManager.Instance.Gold.ToString()}";
            gameObject.SetActive(false);
        }

        private void HandleCurrency(CurrencyItemSO currency)
        {
            PlayerManager.Instance.AddGold(currency.amount);
        }

        private void HandleEquipment(EquipmentItemSO equipment)
        {
            PlayerManager.Instance.equipmentInventory.Add(equipment);
        }
    }
}