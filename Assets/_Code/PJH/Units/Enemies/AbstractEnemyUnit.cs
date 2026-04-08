using System.Linq;
using Code.Core.Debugs;
using Code.Core.Events.Bus;
using Code.Managers;
using Code.Map;
using Code.SkillSystem;
using Code.UnitSystem.Enemies.AI;
using Code.UnitSystem.UnitComponent;
using Code.Utils;
using GondrLib.Dependencies;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Code.UnitSystem.Enemies
{
    public abstract class AbstractEnemyUnit : Unit
    {
        public BehaviorGraphAgent BTAgent { get; private set; }
        public PathMover PathMover { get; private set; }
        public EnemySkillComponent SkillCompo { get; private set; }
        public TurnChannel TurnChannel { get; private set; }
        public UnitAnimation UnitAnimator { get; private set; }
        public UnitRotation UnitRotationCompo { get; private set; }
        public UnitAnimationTrigger AnimationTrigger { get; private set; }
        public EnemyManager EnemyManager => _enemyManager;
        protected GridMap GridMapInstance { get; private set; }
        protected UnitManager UnitManager { get; private set; }
        protected Unit CurrentTarget { get; private set; }

        [Inject] protected EnemyManager _enemyManager;

        private bool _hasEndedTurn;
        private bool _isDead;
        
        private readonly Vector3 _dampingSpeed = new(1.5f, 1.5f, 1.5f);

        protected override void Awake()
        {
            base.Awake();
            
            BTAgent = GetComponent<BehaviorGraphAgent>();
        }

        protected override void AfterInitComponents()
        {
            base.AfterInitComponents();
            
            PathMover = GetUnitCompo<PathMover>();
            SkillCompo = GetUnitCompo<EnemySkillComponent>();
            UnitAnimator = GetUnitCompo<UnitAnimation>();
            UnitRotationCompo = GetUnitCompo<UnitRotation>();
            AnimationTrigger = GetUnitCompo<UnitAnimationTrigger>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _isDead = false;

            if (AnimationTrigger != null)
                AnimationTrigger.OnDeadEvent += HandleDeathAnimEnd;
        }

        protected override void OnDisable()
        {
            if (AnimationTrigger != null)
                AnimationTrigger.OnDeadEvent -= HandleDeathAnimEnd;

            base.OnDisable();
        }

        protected virtual void Start()
        {
            SetVariableValue(BTVars.UnitAnimator, UnitAnimator);

            if (GetVariableValue(BTVars.TurnChannel, out BlackboardVariable<TurnChannel> targetChannel))
                TurnChannel = targetChannel.Value;

            GridMapInstance = GridMap.Instance;
            UnitManager = FindFirstObjectByType<UnitManager>();
            UpdateTargetBlackboard();
        }

        public override void OnTurnStart()
        {
            _hasEndedTurn = false;
            base.OnTurnStart();

            if (!PrepareTurnStart())
            {
                StartCoroutine(EndTurnNextFrame());
                return;
            }
            
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(gameObject, false, _dampingSpeed));

            TurnChannel?.SendEventMessage();
        }

        private IEnumerator EndTurnNextFrame()
        {
            yield return null;

            if (this != null && gameObject.activeInHierarchy)
                OnTurnEnd();
        }

        public override void OnTurnEnd()
        {
            if (_hasEndedTurn)
                return;

            _hasEndedTurn = true;
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null,
                false, new Vector3(0.1f, 0.1f, 0.1f)));
            
            base.OnTurnEnd();
        }

        protected override void Dead()
        {
            if (_isDead)
                return;

            _isDead = true;
            base.Dead();

            if (UnitAnimator == null || AnimationTrigger == null)
            {
                HandleDeathAnimEnd();
                return;
            }

            UnitAnimator.PlaySelectAnimation("DIE");
        }

        protected override void Hit()
        {
            if (_isDead)
                return;

            if (UnitAnimator != null)
            {
                UnitAnimator.RestartFromEntry();
                UnitAnimator.PlaySelectAnimation("HIT");
            }

            base.Hit();
        }

        private void HandleDeathAnimEnd()
        {
            if (!_isDead)
                return;

            _isDead = false;

            if (Core.Managers.StageManager.Instance != null)
                Core.Managers.StageManager.Instance.RemoveEnemy(gameObject);

            gameObject.SetActive(false);
        }

        protected virtual bool PrepareTurnStart()
            => UpdateTargetBlackboard();

        public void OrderSkill(SkillSO skillSO, GameObject target, System.Action onComplete)
        {
            if (!TryGetSkill(skillSO, out SkillSO selectedSkillSO, out BaseSkill selectedSkill))
            {
                onComplete?.Invoke();
                return;
            }

            EnemyAttack(selectedSkillSO, selectedSkill, target, onComplete);
        }

        private bool TryGetSkill(SkillSO skillSO, out SkillSO selectedSkillSO, out BaseSkill selectedSkill)
        {
            selectedSkillSO = null;
            selectedSkill = null;

            if (SkillCompo?.Skills == null || SkillCompo.Skills.Count == 0)
            {
                UnityLogger.LogError($"[{nameof(AbstractEnemyUnit)}] {name} has no registered skills.");
                return false;
            }

            if (skillSO != null && SkillCompo.Skills.TryGetValue(skillSO, out BaseSkill exactSkill) && exactSkill != null)
            {
                selectedSkillSO = skillSO;
                selectedSkill = exactSkill;
                return true;
            }

            foreach (var pair in SkillCompo.Skills)
            {
                if (pair.Key == null || pair.Value == null)
                    continue;

                selectedSkillSO = pair.Key;
                selectedSkill = pair.Value;
                return true;
            }

            UnityLogger.LogError($"[{nameof(AbstractEnemyUnit)}] {name} could not resolve a skill to execute.");
            return false;
        }

        private void EnemyAttack(SkillSO skillSO, BaseSkill skill, GameObject target, System.Action onComplete)
        {
            UnityAction endListener = null;
            endListener = () =>
            {
                skill.SkillEndEvent?.RemoveListener(endListener);
                onComplete?.Invoke();
            };

            skill.SkillEndEvent?.AddListener(endListener);
            skill.RotationCompo = UnitRotationCompo;
            skill.ConfigureSkillRange(skillSO);
            skill.ForceUseSkill(target);
        }

        public bool CanUseSkillOnTarget(SkillSO skillSO, GameObject target)
        {
            if (target == null || SkillCompo?.Skills == null || SkillCompo.Skills.Count == 0)
            {
                UnityLogger.LogError($"[{nameof(AbstractEnemyUnit)}] {name} cannot check skill range without target or skills.");
                return false;
            }

            if (!TryGetSkill(skillSO, out _, out var selectedSkill))
                return false;

            if (selectedSkill is not EnemyBaseSkill enemySkill)
            {
                UnityLogger.LogError($"[{nameof(AbstractEnemyUnit)}] {name} tried to evaluate a non-enemy skill.");
                return false;
            }

            return enemySkill.CanUseOnTarget(target);
        }

        public bool TrySelectAttackSkill(GameObject target, out SkillSO selectedSkillSO)
        {
            if (EnemyManager != null)
                return EnemyManager.TrySelectAttackSkill(this, target, out selectedSkillSO);

            selectedSkillSO = null;
            return false;
        }

        protected virtual bool UpdateTargetBlackboard()
        {
            EnemyPlan plan = EnemyManager?.BuildPlan(this);
            CurrentTarget = plan?.Target ?? FindClosestPlayerTarget();
            SetVariableValue(BTVars.Target, CurrentTarget != null ? CurrentTarget.gameObject : null);
            return CurrentTarget != null;
        }

        protected virtual Unit FindClosestPlayerTarget()
        {
            if (GridMapInstance == null || UnitManager == null)
                return null;

            Vector2Int myPos = GridMapInstance.WorldToGridPosition(transform.position);

            return UnitManager.GetPlayerUnits()
                .Where(unit => unit != null && unit.gameObject.activeInHierarchy)
                .OrderBy(unit => DistanceUtils.GetEuclideanDistance(myPos,
                    GridMapInstance.WorldToGridPosition(unit.transform.position)))
                .FirstOrDefault();
        }

        public void SetVariableValue<T>(string variableName, T value)
        {
            Debug.Assert(!string.IsNullOrEmpty(variableName), "Variable name is empty");

            if (BTAgent.GetVariable(variableName, out BlackboardVariable<T> variable))
                variable.Value = value;
            else
                UnityLogger.LogError($"Variable {variableName} not found");
        }

        public bool GetVariableValue<T>(string variableName, out BlackboardVariable<T> variable)
        {
            Debug.Assert(!string.IsNullOrEmpty(variableName), "Variable name is empty");
            return BTAgent.GetVariable(variableName, out variable);
        }
    }
}
