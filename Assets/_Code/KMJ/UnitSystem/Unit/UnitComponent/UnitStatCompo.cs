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
    }
    public class UnitStatCompo : MonoBehaviour, IUnitComponent
    {
        private UnitSO unitSO;

        public float MoveSpeed => unitSO.MoveSpeed;

        public float AtkDamage => unitSO.AtkDamage;

        public float MaxHealth => unitSO.Maxhealth;

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
                _ => default(T)
            };
            
            if (value is T typedValue)
                return typedValue;

            return default;
        }
    }
}