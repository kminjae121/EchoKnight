using Code.Core.Events.Bus;
using Code.UnitManaging;
using Code.UnitSystem;
using UnityEngine;

namespace UnitSystem
{
    public class UnitCostComponent : MonoBehaviour, IUnitComponent
    {
        private Unit unit;
        
        public void Initialize(Unit owner)
        {
            unit = owner;
        }
        
        public bool GetCost(int cost)
        {
            if (OwnUnitManage.Instance == null) return false;
            if (OwnUnitManage.Instance.currentCost >= 100 || OwnUnitManage.Instance.currentCost + cost >= 100)
                return false;

            OwnUnitManage.Instance.currentCost += cost;

            if (OwnUnitManage.Instance.currentCost >= 100)
                OwnUnitManage.Instance.currentCost = 100;
            
            UpdateAPGauge();
            return true;
        }

        public float GetCurrentCost()
        {
            return OwnUnitManage.Instance != null ? OwnUnitManage.Instance.currentCost : 0;
        }

        public void RemoveCost(float cost)
        {
            if (OwnUnitManage.Instance == null) return;

            OwnUnitManage.Instance.currentCost -= cost;
            if (OwnUnitManage.Instance.currentCost <= 0) OwnUnitManage.Instance.currentCost = 0;
            
            UpdateAPGauge();
        }
        
        public void UpdateAPGauge()
        {
            if (OwnUnitManage.Instance == null) return;
            float value = Mathf.Clamp01(OwnUnitManage.Instance.currentCost / 100);
            
            Bus<ActionGaugeEvent>.Raise(new ActionGaugeEvent(value));
        }
    }
}