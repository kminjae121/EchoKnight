using _Code.UnitSystem;
using UnityEngine;

namespace Code.UnitSystem
{
    public enum StatInfo
    {
        MoveSpeed, 
        AtkDamage,
        MaxHealth,
        DefensivePower,
        AvoidProbability,
        CriticalProbability,
        CriticalIncreaseValue,
    }
    public class UnitStatCompo : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private UnitSO unitSO;

        private float MoveSpeed => unitSO.MoveSpeed;

        private float MaxHealth => unitSO.Maxhealth;
        
        private float AttackDamage => unitSO.AttackDamage;
        
        private float DefensivePower => unitSO.DefensivePower;
        
        private float AvoidProbability => unitSO.AvoidProbability;

        private float CriticalProbability => unitSO.CriticalProbability;

        private float CriticalIncreaseValue => unitSO.CriticalDamageIncrease;

        public void Initialize(Unit owner)
        {
            if(unitSO == null)
                unitSO = owner.unitSO;
        }

        public float GetStat(StatInfo statInfo)
        {
            float value = 0f;
            
            switch (statInfo)
            {
                case StatInfo.MoveSpeed:
                    value = MoveSpeed;
                    break;
                case StatInfo.MaxHealth:
                    value = MaxHealth;
                    break;
                case StatInfo.AtkDamage:
                    value =AttackDamage;
                    break;
                case StatInfo.DefensivePower:
                    value = DefensivePower;
                    break;
                case StatInfo.AvoidProbability:
                    value = AvoidProbability;
                    break;
                case  StatInfo.CriticalProbability:
                    value = CriticalProbability;
                    break;
                case StatInfo.CriticalIncreaseValue:
                    value = CriticalIncreaseValue;
                    break;
            }
            
            value += InGameStatCompo.Instance.GetStat(statInfo, unitSO.UnitType);
            
            return (float)value;
        }
    }
}