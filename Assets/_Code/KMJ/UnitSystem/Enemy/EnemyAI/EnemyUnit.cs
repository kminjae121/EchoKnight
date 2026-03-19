using System;
using System.Collections;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Map;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        
        public IMapTile currentTile { get; set; }

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
            if (_mover != null)
            {
                _mover.Initialize(this);
            }

            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
            OnIdleRequested();
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();
            if (_ai != null) _ai.SetTurnState(true);
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(gameObject,
                true, new Vector3(0.1f, 0.1f, 0.1f)));
        }

        public override void OnTurnEnd()
        {
            if (_ai != null) _ai.SetTurnState(false);
            base.OnTurnEnd();
            Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
            
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, 
                false,new Vector3(0.1f,0.1f,0.1f)));
        }

        #region [Commands]

        public void OrderMove(Vector3 targetPos, int maxSteps, Action onComplete)
        {
            if (_mover == null)
            {
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

        public void OrderSkill(SkillSO skillso, GameObject target, Action onComplete)
        {
            if (_skillCompo == null)
            {
                Debug.LogError($"[EnemyUnit] {name}에게 SkillComponent가 없습니다.");
                onComplete?.Invoke();
                return;
            }

            BaseSkill skillToUse = null;

            if (!string.IsNullOrEmpty(skillso.skillName) && _skillCompo.skills.ContainsKey(skillso))
            {
                skillToUse = _skillCompo.skills[skillso];
            }
            else if (_skillCompo.skills.Count > 0)
            {
                var enumerator = _skillCompo.skills.Values.GetEnumerator();
                if (enumerator.MoveNext()) 
                {
                    skillToUse = enumerator.Current;
                    Debug.LogWarning($"[EnemyUnit] '{skillso.skillName}' 스킬을 찾지 못해 '{skillToUse.GetType().Name}'(으)로 대체하여 실행합니다.");
                }
            }

            if (skillToUse != null)
            {
                StartCoroutine(ProcessSkillRoutine(skillToUse, target, onComplete));
            }
            else
            {
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
                Debug.LogWarning($"[EnemyUnit] {name} 스킬 타임아웃.");
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
            if (Code.Core.Managers.StageManager.Instance != null)
                Code.Core.Managers.StageManager.Instance.RemoveEnemy(this.gameObject);
        }
    }
}