using System.Linq;
using Code.Core.Debugs;
using Code.Managers;
using Code.Map;
using Code.SkillSystem;
using Code.UnitSystem.Enemies.AI;
using Code.UnitSystem.UnitComponent;
using Code.Utils;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Events;

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
        protected GridMap GridMapInstance { get; private set; }
        protected UnitManager UnitManager { get; private set; }
        protected Unit CurrentTarget { get; private set; }

        private bool _hasEndedTurn;

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
                OnTurnEnd();
                return;
            }

            TurnChannel?.SendEventMessage();
        }

        public override void OnTurnEnd()
        {
            if (_hasEndedTurn)
                return;

            _hasEndedTurn = true;
            base.OnTurnEnd();
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

            if (!TryGetSkill(skillSO, out var selectedSkillSO, out var selectedSkill))
                return false;

            if (selectedSkill is FrontPierceEnemyAttack pierceAttack)
                return pierceAttack.CanHitTarget(target);

            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                return false;

            Vector2Int myPos = gridMap.WorldToGridPosition(transform.position);
            Vector2Int targetPos = gridMap.WorldToGridPosition(target.transform.position);

            float distance = DistanceUtils.GetEuclideanDistance(myPos, targetPos);
            float range = Mathf.Max(0f, selectedSkillSO.SkillRange);

            return distance <= range;
        }

        public bool TrySelectAttackSkill(GameObject target, out SkillSO selectedSkillSO)
        {
            selectedSkillSO = null;

            if (target == null || SkillCompo?.Skills == null || SkillCompo.Skills.Count == 0)
                return false;

            SkillSO bestPierceSkill = null;
            int bestPierceHitCount = 0;
            SkillSO basicSkill = null;
            SkillSO fallbackSkill = null;

            foreach (var pair in SkillCompo.Skills)
            {
                SkillSO skillSO = pair.Key;
                BaseSkill skill = pair.Value;

                if (skillSO == null || skill == null)
                    continue;

                if (!CanUseSkillOnTarget(skillSO, target))
                    continue;

                fallbackSkill ??= skillSO;

                if (skill is FrontPierceEnemyAttack pierceSkill)
                {
                    int hitCount = pierceSkill.GetPredictedHitCount(target);

                    if (hitCount > bestPierceHitCount)
                    {
                        bestPierceHitCount = hitCount;
                        bestPierceSkill = skillSO;
                    }

                    continue;
                }

                if (skillSO.SkillType == SkillType.BasicSkill)
                    basicSkill ??= skillSO;
            }

            if (bestPierceSkill != null && bestPierceHitCount >= 2)
            {
                selectedSkillSO = bestPierceSkill;
                return true;
            }

            if (basicSkill != null)
            {
                selectedSkillSO = basicSkill;
                return true;
            }

            if (bestPierceSkill != null)
            {
                selectedSkillSO = bestPierceSkill;
                return true;
            }

            if (fallbackSkill != null)
            {
                selectedSkillSO = fallbackSkill;
                return true;
            }

            return false;
        }

        protected virtual bool UpdateTargetBlackboard()
        {
            CurrentTarget = FindClosestPlayerTarget();
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
