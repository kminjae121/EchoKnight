using System;
using Code.UnitSystem;
using EntityComponent;
using UnityEngine;

namespace UnitSystem
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

       private UnitInGameSO unitInGameSO;

        private float MoveSpeed => unitSO.MoveSpeed;

        private float AtkDamage => unitInGameSO.AtkDamage;

        private float MaxHealth => unitInGameSO.Maxhealth;
        
        private float SkillDamage => unitInGameSO.SkillDamage;
        
        private float DefensivePower => unitInGameSO.DefensivePower;

        public void Initialize(Unit owner)
        {
            unitSO = owner.unitSO;
            unitInGameSO = unitSO.unitInGame;
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