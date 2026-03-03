using System;
using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace EnemySystem
{
    [RequireComponent(typeof(EnemyAI))]
    [RequireComponent(typeof(EnemyGridMovingSystem))]
    [RequireComponent(typeof(SkillComponent))]
    public class EnemyUnit : Unit
    {
        [Header("Enemy Modules")]
        [SerializeField] private EnemyAI _ai;
        [SerializeField] private EnemyGridMovingSystem _mover;
        [SerializeField] private SkillComponent _skillCompo;
        [SerializeField] private UnitAnimationTrigger _animTrigger;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _bloodEffect;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (_ai == null) _ai = GetComponent<EnemyAI>();
            if (_mover == null) _mover = GetComponent<EnemyGridMovingSystem>();
            if (_skillCompo == null) _skillCompo = GetComponent<SkillComponent>();
            
            if (_animTrigger == null) 
                _animTrigger = GetComponentInChildren<UnitAnimationTrigger>();

            if (_animTrigger != null)
            {
                _animTrigger.OnEnemyAnimationEndTrigger += OnIdleRequested;
                _animTrigger.OnEnemyDieEndTrigger += OnDeathAnimationFinished;
            }
        }

        protected override void OnDisable()
        {
            if (_animTrigger != null)
            {
                _animTrigger.OnEnemyAnimationEndTrigger -= OnIdleRequested;
                _animTrigger.OnEnemyDieEndTrigger -= OnDeathAnimationFinished;
            }
            
            base.OnDisable();
        }

        private void Start()
        {
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
            OnIdleRequested();
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();
            if (_ai != null) _ai.SetTurnState(true);
        }

        public override void OnTurnEnd()
        {
            if (_ai != null) _ai.SetTurnState(false);
            base.OnTurnEnd();
            Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
        }

        #region [Commands]

        public void OrderMove(Vector3 targetPos, int maxSteps, Action onComplete)
        {
            if (_mover == null)
            {
                Debug.LogWarning($"[EnemyUnit] {name}에게 EnemyGridMovingSystem이 없습니다.");
                onComplete?.Invoke();
                return;
            }

            if (AnimationCompo != null) 
                AnimationCompo.PlaySelectAnimation("MOVE");
            
            _mover.MoveTo(targetPos, maxSteps, () =>
            {
                OnIdleRequested();
                onComplete?.Invoke();
            });
        }

        public void OrderRetreat(Vector3 targetPos, int steps, Action onComplete)
        {
            if (_mover == null)
            {
                onComplete?.Invoke();
                return;
            }
            
            if (AnimationCompo != null) 
                AnimationCompo.PlaySelectAnimation("RETREAT");

            _mover.RetreatFromTarget(targetPos, steps, () =>
            {
                OnIdleRequested();
                onComplete?.Invoke();
            });
        }

        public void OrderSkill(string skillName, GameObject target, Action onComplete)
        {
            if (_skillCompo == null)
            {
                Debug.LogError($"[EnemyUnit] {name}에게 SkillComponent가 없습니다.");
                onComplete?.Invoke();
                return;
            }

            BaseSkill skillToUse = null;

            if (!string.IsNullOrEmpty(skillName) && _skillCompo.skills.ContainsKey(skillName))
            {
                skillToUse = _skillCompo.skills[skillName];
            }
            else if (_skillCompo.skills.Count > 0)
            {
                var enumerator = _skillCompo.skills.Values.GetEnumerator();
                if (enumerator.MoveNext()) 
                {
                    skillToUse = enumerator.Current;
                    Debug.LogWarning($"[EnemyUnit] '{skillName}' 스킬을 찾지 못해 '{skillToUse.GetType().Name}'(으)로 대체하여 실행합니다.");
                }
            }

            if (skillToUse != null)
            {
                StartCoroutine(ProcessSkillRoutine(skillToUse, target, onComplete));
            }
            else
            {
                Debug.LogError($"[EnemyUnit] {name}: 실행할 수 있는 스킬이 없습니다. (SkillComponent 초기화 실패 또는 SkillSO 설정 확인 필요)");
                onComplete?.Invoke();
            }
        }

        private IEnumerator ProcessSkillRoutine(BaseSkill skill, GameObject target, Action onComplete)
        {
            bool isSkillEnded = false;
            
            UnityEngine.Events.UnityAction endListener = () => 
            {
                isSkillEnded = true;
            };
            
            string debugSkillName = skill.GetType().Name;

            if (skill.skillEndEvent != null)
                skill.skillEndEvent.AddListener(endListener);
            
            skill.ForceUseSkill(target);

            float timeout = 3.0f; 
            float timer = 0f;

            while (!isSkillEnded && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            if (timer >= timeout)
            {
                Debug.LogWarning($"[EnemyUnit] {name}의 스킬 '{debugSkillName}' 실행 시간이 초과되어 강제 종료합니다. (Animation Event 누락 확인 필요)");
            }

            if (skill.skillEndEvent != null)
                skill.skillEndEvent.RemoveListener(endListener);
            
            OnIdleRequested();
            onComplete?.Invoke();
        }

        #endregion

        private void OnIdleRequested()
        {
            if (AnimationCompo != null)
                AnimationCompo.PlaySelectAnimation("IDLE");
        }

        protected override void Hit()
        {
            if (_bloodEffect != null)
            {
                _bloodEffect.gameObject.SetActive(true);
                _bloodEffect.Play();
            }

            if (AnimationCompo != null)
            {
                AnimationCompo.RestartFromEntry();
                AnimationCompo.PlaySelectAnimation("HIT");
            }
            base.Hit();
        }

        protected override void Dead()
        {
            if (AnimationCompo != null)
                AnimationCompo.PlaySelectAnimation("DIE");
            
            base.Dead();
        }

        private void OnDeathAnimationFinished()
        {
            gameObject.SetActive(false);
            if (_Code.Core.Managers.StageManager.Instance != null)
                _Code.Core.Managers.StageManager.Instance.RemoveEnemy(this.gameObject);
        }
    }
}