using Code.Core.Events.Bus;
using Code.UI;
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
        
        private Unit _entity;
        private ActionData _actionData;
        private UnitStatCompo _statCompo;
        private UnitState unitStateCompo;
        
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
        }
        
        private void Start()
        {
            maxHealth = currentHealth = _statCompo.GetStat(StatInfo.MaxHealth);
            _defensivePower =  _statCompo.GetStat(StatInfo.DefensivePower);
             unitStateCompo = new UnitState(_entity.unitSO);
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
                
                unitStateCompo.Heal(amount);
            }
        }
        

        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, AttackDataSO attackData, Unit dealer)
        {
            _actionData.HitNormal = hitNormal;
            _actionData.HitPoint = hitPoint;
            _actionData.HitByPowerAttack = attackData.isPowerAttack;
            _actionData.LastDamageData = damageData; 

            _defensivePower = _entity.AddDefensivePower;

            float damage = damageData.damage;
            float CalculateDamage = damage * (_defensivePower / 100);
            damage -= CalculateDamage;
            
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

            OnHealthChangedEvent?.Invoke(currentHealth, maxHealth);
            
            int typeHash = damageData.isCritical ? criticalText.nameHash : normalText.nameHash;
            Vector3 position = hitPoint + new Vector3(0, 1.2f);
            PopupTextEvent textEvt = TextEvent.PopupTextEvent.Initializer(damage.ToString(), typeHash
                , position, 0.5f);  
            
            textEventChannel.RaiseEvent(textEvt);
           
           if (_entity as CharacterUnit)
           {
               CharacterUnit characterUnit = _entity as CharacterUnit;
               
               Bus<SetUpUnitHealthBar>.Raise(new SetUpUnitHealthBar(characterUnit.PlayableUnitID,CurrentHealth,
                   MaxHealth, characterUnit.UnitImage));

               unitStateCompo.TakeDamage(damage);
           }
           
           _entity.OnHitEvent?.Invoke(); //이벤트만 발행한다.
           
           if (currentHealth <= 0)
               _entity.OnDeathEvent?.Invoke();
        }
    }
}