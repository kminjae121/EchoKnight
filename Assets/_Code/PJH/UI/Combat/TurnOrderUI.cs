using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class TurnOrderUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TurnManager turnManager;
        
        [Header("Settings")]
        [SerializeField] private int showTurnOrderCount = 5; 
        
        [Header("UI Elements")]
        [SerializeField] private List<Image> turnOrderImages;
        
        private void OnEnable()
        {
            Bus<TurnOrderUpdateEvent>.Subscribe(HandleTurnOrderUpdate);
        }

        private void OnDisable()
        {
            Bus<TurnOrderUpdateEvent>.Unsubscribe(HandleTurnOrderUpdate);
        }

        private void HandleTurnOrderUpdate(TurnOrderUpdateEvent evt)
        {
            var units = turnManager.GetTimelineUnits(showTurnOrderCount);
            
            for (int i = 0; i < showTurnOrderCount; ++i)
                if (i < units.Count)
                {
                    turnOrderImages[i].sprite = units[i].UnitImage;
                    turnOrderImages[i].enabled = true;
                }
                else
                    turnOrderImages[i].enabled = false;
        }
    }
}