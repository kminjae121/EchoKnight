using System.Globalization;
using GameEventChannel;
using UnitSystem;
using UnityEngine;
using TextInfo = UI.TextInfo;

namespace EntityComponent
{
    public class EntityHealth : MonoBehaviour, IUnitComponent, IDamageable, IAfterInitialize
    {
        private Unit _entity;
        private ActionData _actionData;
        private EntityStatCompo _statCompo;

        [SerializeField] private StatSO hpStat;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private TextInfo normalText, criticalText;
        [SerializeField] private GameEventChannelSO textEventChannel;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        
        
        public delegate void OnHealthChanged(float current, float max);

        public event OnHealthChanged OnHealthChangedEvent;
        
        public void Initialize(Unit owner)
        {
            _entity = owner;
            _actionData = owner.GetUnitCompo<ActionData>();
            _statCompo = owner.GetUnitCompo<EntityStatCompo>();
        }

        public void AfterInitialize()
        {
            maxHealth = currentHealth = _statCompo.SubscribeStat(
                hpStat, HandleMaxHPChanged, 10f);
        }

        private void OnDestroy()
        {
            _statCompo.UnSubscribeStat(hpStat, HandleMaxHPChanged);
        }

        private void HandleMaxHPChanged(StatSO stat, float currentvalue, float previousvalue)
        {
            float changed = currentvalue - previousvalue; 
            maxHealth = currentvalue;
            if (changed > 0)
                currentHealth = Mathf.Clamp(currentHealth + changed, 0, maxHealth);
            else
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, AttackDataSO attackData, Unit dealer)
        {
            
            _actionData.HitNormal = hitNormal;
            _actionData.HitPoint = hitPoint;
            _actionData.HitByPowerAttack = attackData.isPowerAttack;
            _actionData.LastDamageData = damageData; //데미지 데이터도 기록
            //넉백은 나중에 처리한다.

            currentHealth = Mathf.Clamp(currentHealth - damageData.damage, 0, maxHealth);

            OnHealthChangedEvent?.Invoke(currentHealth, maxHealth);
            
            int typeHash = damageData.isCritical ? criticalText.nameHash : normalText.nameHash;
            Vector3 position = hitPoint + new Vector3(0, 0.3f);
            PopupTextEvent textEvt = TextEvent.PopupTextEvent.Initializer(damageData.damage.ToString(), typeHash
                , position, 0.5f);  
            
            textEventChannel.RaiseEvent(textEvt);
           if (currentHealth <= 0)
           {
               _entity.OnDeathEvent?.Invoke();
           }
           
           _entity.OnHitEvent?.Invoke(); //이벤트만 발행한다.
        }

    }
}