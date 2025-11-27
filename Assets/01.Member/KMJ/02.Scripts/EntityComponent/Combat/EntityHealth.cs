using System.Globalization;
using GameEventChannel;
using UnitSystem;
using UnityEngine;
using TextInfo = UI.TextInfo;

namespace EntityComponent
{
    public class EntityHealth : MonoBehaviour, IEntityComponent, IDamageable, IAfterInitialize
    {
        private Entity _entity;
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

        public void Initialize(Entity entity)
        {
            _entity = entity;
            _actionData = entity.GetCompo<ActionData>();
            _statCompo = entity.GetCompo<EntityStatCompo>();
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
            float changed = currentvalue - previousvalue; //얼마만큼 변했는지를 측정
            maxHealth = currentvalue;
            if (changed > 0)
                currentHealth = Mathf.Clamp(currentHealth + changed, 0, maxHealth);
            else
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            //현재 체력에는 변한값만큼 더해준다.(안해줘도 상관없다. 안하면 최대체력 증가시 체력감소가 된다.)
        }

        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, AttackDataSO attackData, Entity dealer)
        {
            _actionData.HitNormal = hitNormal;
            _actionData.HitPoint = hitPoint;
            _actionData.HitByPowerAttack = attackData.isPowerAttack;
            _actionData.LastDamageData = damageData; //데미지 데이터도 기록
            //넉백은 나중에 처리한다.

            currentHealth = Mathf.Clamp(currentHealth - damageData.damage, 0, maxHealth);

            OnHealthChangedEvent?.Invoke(currentHealth, maxHealth);
            
            int typeHash = damageData.isCritical ? criticalText.nameHash : normalText.nameHash;
            Vector3 position = hitPoint + new Vector3(0, 1.5f);
            PopupTextEvent textEvt = TextEvent.PopupTextEvent.Initializer(damageData.damage.ToString(), typeHash
                , position, 0.5f);  
            textEventChannel.RaiseEvent(textEvt);
            //크리티컬 처리는 나중에.
            if (currentHealth <= 0)
            {
                _entity.OnDeathEvent?.Invoke();
            }
            
            _entity.OnHitEvent?.Invoke(); //이벤트만 발행한다.
        }

        
    }
}