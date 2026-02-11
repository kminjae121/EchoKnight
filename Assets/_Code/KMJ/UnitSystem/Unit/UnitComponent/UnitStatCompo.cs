using Code.UnitSystem;
using EntityComponent;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.Unit.UnitComponent
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
        private UnitSO unitSO;

        private float MoveSpeed => unitSO.MoveSpeed;

        private float AtkDamage => unitSO.AtkDamage;

        private float MaxHealth => unitSO.Maxhealth;
        
        private float SkillDamage => unitSO.SkillDamage;
        
        private float DefensivePower => unitSO.DefensivePower;

        public void Initialize(Code.UnitSystem.Unit owner)
        {
            unitSO = owner.unitSO;
        }
        
        public T GetStat<T>(StatInfo statInfo)
        {
            object value = statInfo switch
            {
                StatInfo.MoveSpeed => MoveSpeed,
                StatInfo.AtkDamage => AtkDamage,
                StatInfo.MaxHealth => MaxHealth,
                StatInfo.SkillDamage => SkillDamage,
                StatInfo.DefensivePower => DefensivePower,
                _ => default(T)
            };
            
            if (value is T typedValue)
                return typedValue;

            return default;
        }
    }
}