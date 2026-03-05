using System;
using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class TurnEndUI : MonoBehaviour
    {
        [SerializeField] private GameObject turnEndUI;
        [SerializeField] private Button _btn;

        private void Awake()
        {
            //Bus<TurnEndUIEvent>.Subscribe(ActiveTurnUI);
        }

        private void OnDestroy()
        {
            //Bus<TurnEndUIEvent>.Unsubscribe(ActiveTurnUI);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                _btn.onClick?.Invoke();
            }
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