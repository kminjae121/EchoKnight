using System;
using _Code.Combat;
using Code.Core.Events.Bus;
using Code.UI;
using Code.UnitManaging;
using EntityComponent;
using GameEventChannel;
using UnityEngine;

namespace Code.UnitSystem.Combat
{
    public class UnitHealth : MonoBehaviour, IUnitComponent, IDamageable
    {
        [SerializeField] private StatSO hpStat;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private TextInfo normalText, criticalText;
        [SerializeField] private GameEventChannelSO textEventChannel;

        [SerializeField] private UnitStorageSO storageSO; 

        private Unit _entity;
        private ActionData _actionData;
        private UnitStatCompo _statCompo;
        private UnitState _unitStateCompo;
        private UnitShieldCompo _shieldCompo;
        
        private float _defensivePower;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        
        public delegate void OnHealthChanged(float current, float max);
        public event OnHealthChanged OnHealthChangedEvent;
        
        public void Initialize(Unit owner)
        {
            _entity = owner;
            _actionData = owner.GetUnitCompo<ActionData>();
            _statCompo = owner.GetUnitCompo<UnitStatCompo>();
            if(_entity as CharacterUnit)
                _shieldCompo = owner.GetUnitCompo<UnitShieldCompo>();
        }
        
        private void Start()
        {
            _defensivePower =  _statCompo.GetStat(StatInfo.DefensivePower);
            
            if (_entity as CharacterUnit)
            {
                foreach (var unitState in storageSO.unitStates)
                {
                    if(unitState.Data == _entity.unitSO)
                        _unitStateCompo = unitState;
                }
                maxHealth = currentHealth = _unitStateCompo.CurrentHp.Value;   
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                DamageData damage = new DamageData();
                damage.damage = 3;
                ApplyDamage(damage, transform.position, transform.position, null, null,false);
            }
        }

        public void HealHp(float amount)
        {
            currentHealth += amount;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            
            if (_entity as CharacterUnit)
            {
                CharacterUnit characterUnit = _entity as CharacterUnit;
               
                Bus<SetUpUnitHealthBar>.Raise(new SetUpUnitHealthBar(characterUnit.PlayableUnitID,CurrentHealth
                    ,MaxHealth, characterUnit.UnitImage));
                
                _unitStateCompo.Heal(amount);
            }
        }
        

        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, AttackDataSO attackData, Unit dealer,bool isCritical)
        {
            _actionData.HitNormal = hitNormal;
            _actionData.HitPoint = hitPoint;
            _actionData.LastDamageData = damageData;

            if (_entity as CharacterUnit && _shieldCompo.GetShieldValue() > 0)
            {
                _shieldCompo.BreakShield((int)damageData.damage);
                return;
            }

            _defensivePower = _entity.unitSO.DefensivePower;

            float damage = damageData.damage;
            float CalculateDamage = damage * (_defensivePower / 100);
            damage -= CalculateDamage;
            
            currentHealth = Mathf.Clamp(currentHealth - (int)damage, 0, maxHealth);

            OnHealthChangedEvent?.Invoke(currentHealth, maxHealth);
            
            int typeHash = isCritical ? criticalText.nameHash : normalText.nameHash;
            Vector3 position = hitPoint + new Vector3(0, 1.2f);
            PopupTextEvent textEvt = TextEvent.PopupTextEvent.Initializer(damage.ToString(), typeHash
                , position, 0.5f);  
            
            textEventChannel.RaiseEvent(textEvt);
           
           if (_entity as CharacterUnit)
           {
               CharacterUnit characterUnit = _entity as CharacterUnit;
               
               Bus<SetUpUnitHealthBar>.Raise(new SetUpUnitHealthBar(characterUnit.PlayableUnitID,CurrentHealth,
                   MaxHealth, characterUnit.UnitImage));

               _unitStateCompo.TakeDamage(damage);
           }
           
           _entity.OnHitEvent?.Invoke();
           
           if (currentHealth <= 0)
               _entity.OnDeathEvent?.Invoke();
        }
    }
}