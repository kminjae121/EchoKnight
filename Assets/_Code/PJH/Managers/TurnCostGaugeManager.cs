using Code.Core;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Managers
{
    public class TurnCostGaugeManager : MonoBehaviour
    {
        [SerializeField] private int addGaugeValue;
        
        public int maxGaugeValue;
        public NotifyValue<int> currentGaugeValue;

        private void OnEnable()
        {
            Bus<UnitTurnEndEvent>.Subscribe(HandleEndTurn);
        }

        private void OnDisable()
        {
            Bus<UnitTurnEndEvent>.Unsubscribe(HandleEndTurn);
        }

        private void HandleEndTurn(UnitTurnEndEvent evt)
        {
            currentGaugeValue.Value = Mathf.Clamp(currentGaugeValue.Value + addGaugeValue, 0, maxGaugeValue);
        }
        
        public bool CanUseSkill(int skillValue)
            => currentGaugeValue.Value >= skillValue;

        #region Test

        [ContextMenu("RaiseTurnEndEvent")]
        private void RaiseTurnEndEvent()
        {
            Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(null));
        }

        [ContextMenu("AddGaugeValue")]
        private void AddGaugeValue()
        {
            currentGaugeValue.Value += 5;
        }

        #endregion
    }
}