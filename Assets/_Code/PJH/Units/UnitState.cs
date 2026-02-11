using System;
using Code.Core;
using UnityEngine;

namespace Code.UnitSystem
{
    [Serializable]
    public class UnitState
    {
        [field: SerializeField] public UnitSO Data { get; private set; }
        
        public NotifyValue<float> CurrentHp { get; private set; }
        public bool IsDead => CurrentHp.Value <= 0;

        public UnitState(UnitSO data)
        {
            Data = data;
            CurrentHp = new NotifyValue<float>();
            CurrentHp.Value = Data.Maxhealth;
        }
        
        public void TakeDamage(float value)
        {
            if (IsDead)
                return;

            CurrentHp.Value = Mathf.Max(0, CurrentHp.Value - value);
        }

        public void Heal(float value)
        {
            if (IsDead)
                return;

            CurrentHp.Value = Mathf.Min(Data.Maxhealth, CurrentHp.Value + value);
        }
    }
}