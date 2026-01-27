using System;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UI
{
    public class TurnEndUI : MonoBehaviour
    {
        [SerializeField] private GameObject turnEndUI;

        private void Awake()
        {
            Bus<TurnEndUIEvent>.Subscribe(ActiveTurnUI);
        }

        private void OnDestroy()
        {
            Bus<TurnEndUIEvent>.Unsubscribe(ActiveTurnUI);
        }

        public void ActiveTurnUI(TurnEndUIEvent evt)
        {
            if (evt.isActive)
            {
                turnEndUI.SetActive(false);
            }
            else
            {
                turnEndUI.SetActive(true);
            }
        }
    }
}