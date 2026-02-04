using System.Collections;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using EnemySystem;
using EntityComponent;
using GameEventChannel;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class EnemyBasicAttackSkill : BaseSkill
    {
        [Header("Stat Settings")]
        [SerializeField] private StatSO atkDamageStat;
        
        private EntityStatCompo _statCompo;
        private Animator _animator;
        private float _currentAtkDamage;
        
        private readonly int _animIDAttack = Animator.StringToHash("Attack");

        private Coroutine _safetyCoroutine; // 세이프티 코루틴

        public override void InitializeSkill()
        {
            base.InitializeSkill();

            if (_unitBase != null)
            {
                _animator = _unitBase.GetComponentInChildren<Animator>();
                _statCompo = _unitBase.GetComponentInChildren<EntityStatCompo>();
            }
            
            if (_animator == null) 
                _animator = GetComponentInChildren<Animator>();

            if (_statCompo != null && atkDamageStat != null)
            {
                StatSO target = _statCompo.GetStat(atkDamageStat);
                if (target != null)
                {
                    _currentAtkDamage = target.Value;
                    target.OnValueChanged += HandleAtkDamageChanged;
                    _damageData.damage = _currentAtkDamage;
                }
            }

            if (triggerCompo != null)
            {
                triggerCompo.OnAttackTrigger += ApplyDamageToTarget;
            }
            
            skillEvent.AddListener(ExecuteAttackSequence);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            if (_statCompo != null && atkDamageStat != null)
            {
                StatSO target = _statCompo.GetStat(atkDamageStat);
                if (target != null)
                    target.OnValueChanged -= HandleAtkDamageChanged;
            }

            if (triggerCompo != null)
            {
                triggerCompo.OnAttackTrigger -= ApplyDamageToTarget;
            }
            
            skillEvent.RemoveListener(ExecuteAttackSequence);
        }

        private void HandleAtkDamageChanged(StatSO stat, float currentValue, float previousValue)
        {
            _currentAtkDamage += currentValue;
            _damageData.damage = _currentAtkDamage;
        }

        private void ExecuteAttackSequence(GameObject target)
        {
            Debug.Log($"[Check] 스킬 실행 명령 도착! 타겟: {target?.name}");

            // [안전장치] 3초 뒤에 강제 종료하는 타이머 시작
            if (_safetyCoroutine != null) StopCoroutine(_safetyCoroutine);
            _safetyCoroutine = StartCoroutine(ForceEndTimer(3.0f));

            if (_animator != null)
            {
                _animator.SetBool(_animIDAttack, true);
            }
            else
            {
                ApplyDamageToTarget();
            }
        }

        public void ApplyDamageToTarget()
        {
            if (_safetyCoroutine != null) StopCoroutine(_safetyCoroutine);

            if (_animator != null)
            {
                _animator.SetBool(_animIDAttack, false);
            }

            if (_targetEnemy != null) 
            {
                Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f, 0.25f));
                if (impulseSource != null) 
                    impulseSource.GenerateImpulse(0.6f);

                var targetHealth = _targetEnemy.GetComponent<EntityHealth>();
                if (targetHealth != null)
                {
                    targetHealth.ApplyDamage(_damageData, 
                        _targetEnemy.transform.position, 
                        transform.position, 
                        attackData, 
                        _unitBase);
                }
            }

            Debug.Log("[Check] 데미지 적용 완료. 스킬 종료 이벤트 발송.");
            
            skillEnd(); 
            
            skillEndEvent?.Invoke(); 
        }
        
        private IEnumerator ForceEndTimer(float time)
        {
            yield return new WaitForSeconds(time);
            Debug.LogWarning("[Warning] 애니메이션 이벤트 미발생 -> 강제 종료 및 턴 넘김");
            ApplyDamageToTarget(); 
        }
    }
}