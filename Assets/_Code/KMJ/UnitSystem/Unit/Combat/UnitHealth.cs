using System;
using _Code.Combat;
using Code.Core.Events.Bus;
using Code.UI;
using Code.UnitManaging;
using EntityComponent;
using GameEventChannel;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem.Combat
{
    public class UnitHealth : MonoBehaviour, IUnitComponent, IDamageable
    {
        [SerializeField] private StatSO hpStat;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private TextInfo normalText, criticalText, healText;
        [SerializeField] private GameEventChannelSO textEventChannel;

        [field : SerializeField] public UnitStorageSO StorageSO; 

        private Unit _entity;
        private ActionData _actionData;
        private UnitStatCompo _statCompo;
        private UnitState _unitStateCompo;
        private UnitShieldCompo _shieldCompo;
        
        private float _defensivePower;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;


        public UnityEvent<Unit,int> OnInteractionEvent;
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
            _defensivePower = _statCompo.GetStat(StatInfo.DefensivePower);
            
            if (_entity as CharacterUnit)
            {
                foreach (var unitState in StorageSO.unitStates)
                {
                    if(unitState.Data == _entity.unitSO)
                        _unitStateCompo = unitState;
                }

                maxHealth = _unitStateCompo.MaxHealth;
                currentHealth = _unitStateCompo.CurrentHp.Value;   
            }
            else
            {
                maxHealth = currentHealth = _entity.unitSO.Maxhealth;
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

            int healHash = healText.nameHash;

            Vector3 pos = _entity.transform.position + new Vector3(0, 1.2f);;
            
            PopupTextEvent textEvt = TextEvent.PopupTextEvent.Initializer(amount.ToString(), healHash
                , pos, 0.5f);  
            
            textEventChannel.RaiseEvent(textEvt);
        }
        

        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, AttackDataSO attackData,
            Unit dealer,bool isCritical, bool isPenetrate)
        {
            _actionData.HitNormal = hitNormal;
            _actionData.HitPoint = hitPoint;
            _actionData.LastDamageData = damageData;

            int damage = damageData.damage;
            
            if (isPenetrate != true)
            {
                if (_entity as CharacterUnit && _shieldCompo.GetShieldValue() > 0)
                {
                    _shieldCompo.BreakShield((int)damageData.damage);
                    return;
                }

                _defensivePower = _entity.unitSO.DefensivePower;
                
                int CalculateDamage = (int)(damage * (_defensivePower / 100));
                damage -= CalculateDamage;
            
                currentHealth = Mathf.Clamp(currentHealth - (int)damage, 0, maxHealth);   
            }

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
           OnInteractionEvent?.Invoke(dealer, damage);

           if (currentHealth <= 0)
           {
               if(_entity as CharacterUnit)
                    StorageSO.unitStates.Remove(_unitStateCompo);
               
               _entity.OnDeathEvent?.Invoke();
           }
        }
    }
}