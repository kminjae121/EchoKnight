using System;
using System.Collections;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using GameEventChannel;
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

        protected override void OnDestroy()
        {
            if (_animTrigger != null)
            {
                _animTrigger.OnEnemyAnimationEndTrigger -= OnIdleRequested;
                _animTrigger.OnEnemyDieEndTrigger -= OnDeathAnimationFinished;
            }
            base.OnDestroy();
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

            for (int i = 0; i <= 2; i++)
                Bus<SkillUIEvent>.Raise(new SkillUIEvent(i, null, 0, null, null));
        }

        public override void OnTurnEnd()
        {
            if (_ai != null) _ai.SetTurnState(false);
            
            Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
            base.OnTurnEnd();
        }

        #region [Commands]
        
        public void OrderMove(Vector3 targetPos, Action onComplete)
        {
            if (_mover == null)
            {
                onComplete?.Invoke();
                return;
            }

            if (AnimationCompo != null) 
                AnimationCompo.PlaySelectAnimation("MOVE");

            _mover.MoveTo(targetPos, () =>
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
                AnimationCompo.PlaySelectAnimation("MOVE");

            _mover.RetreatFromTarget(targetPos, steps, () =>
            {
                OnIdleRequested();
                onComplete?.Invoke();
            });
        }

        public void OrderSkill(string skillName, GameObject target, Action onComplete)
        {
            if (_skillCompo == null || _skillCompo.skills == null)
            {
                onComplete?.Invoke();
                return;
            }

            if (!_skillCompo.skills.TryGetValue(skillName, out BaseSkill skill))
            {
                var enumerator = _skillCompo.skills.Values.GetEnumerator();
                if (enumerator.MoveNext()) skill = enumerator.Current;
            }

            if (skill != null)
            {
                StartCoroutine(ProcessSkillRoutine(skill, target, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        private IEnumerator ProcessSkillRoutine(BaseSkill skill, GameObject target, Action onComplete)
        {
            bool isSkillEnded = false;
            
            UnityEngine.Events.UnityAction endListener = () => isSkillEnded = true;
            skill.skillEndEvent.AddListener(endListener);

            skill.ForceUseSkill(target);

            while (!isSkillEnded)
            {
                yield return null;
            }

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