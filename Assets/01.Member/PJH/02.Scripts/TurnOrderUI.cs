using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class TurnOrderUI : MonoBehaviour
    {
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private int showTurnOrderCount = 5; 
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
                turnOrderImages[i].sprite = units[0].UnitImage;
        }
    }
}