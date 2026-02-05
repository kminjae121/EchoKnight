using System;
using UnityEngine;

namespace Code.UnitSystem
{
    [Serializable]
    public class UnitState
    {
        public UnitSO Data { get; private set; }
        public float CurrentHp { get; private set; }
        public bool IsDead => CurrentHp <= 0;

        public UnitState(UnitSO data)
        {
            Data = data;
            //CurrentHp
        }
        
        public void TakeDamage(float value)
        {
            if (IsDead)
                return;

            CurrentHp = Mathf.Max(0, CurrentHp - value);
        }

        public void Heal(float value)
        {
            if (IsDead)
                return;
            
            //CurrentHp = Mathf.Min()
        }
    }
}