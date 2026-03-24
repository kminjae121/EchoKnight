using _Code.KMJ.UnitSystem;
using UnityEngine;

namespace Code.UnitSystem
{
    public enum StatInfo
    {
        MoveSpeed, 
        AtkDamage,
        MaxHealth,
        SkillDamage,
        DefensivePower,
    }
    public class UnitStatCompo : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private UnitSO unitSO;

        private float MoveSpeed => unitSO.MoveSpeed;

        private float AtkDamage => unitSO.AtkDamage;

        private float MaxHealth => unitSO.Maxhealth;
        
        private float SkillDamage => unitSO.SkillDamage;
        
        private float DefensivePower => unitSO.DefensivePower;

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
                case StatInfo.AtkDamage:
                    value = AtkDamage;
                    break;
                case StatInfo.MaxHealth:
                    value = MaxHealth;
                    break;
                case StatInfo.SkillDamage:
                    value =SkillDamage;
                    break;
                case StatInfo.DefensivePower:
                    value = DefensivePower;
                    break;
            }
            value += InGameStatCompo.Instance.GetStat(statInfo, unitSO.UnitType);
            
            return (float)value;
        }
    }
}