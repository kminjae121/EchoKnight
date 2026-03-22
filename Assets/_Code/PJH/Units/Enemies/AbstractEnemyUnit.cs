using Code.Core.Debugs;
using Code.UnitSystem.Enemies.AI;
using Code.UnitSystem.UnitComponent;
using Unity.Behavior;
using UnityEngine;
using Action = System.Action;

namespace Code.UnitSystem.Enemies
{
    public abstract class AbstractEnemyUnit : Unit
    {
        public BehaviorGraphAgent BTAgent { get; private set; }
        public PathMover PathMover { get; private set; }
        public TestAttackCompo AttackCompo { get; private set; }
        public TurnChannel TurnChannel { get; private set; }

        public event Action OnTurnEnd;

        protected override void Awake()
        {
            base.Awake();
            BTAgent = GetComponent<BehaviorGraphAgent>();
        }

        protected override void AfterInitComponents()
        {
            base.AfterInitComponents();
            PathMover = GetUnitCompo<PathMover>();
            AttackCompo = GetUnitCompo<TestAttackCompo>(); // 나중에 수정
            // 어택 컴포넌트 가져오기
        }

        protected virtual void Start()
        {
            SetVariableValue<AbstractEnemyUnit>(BTVars.Enemy, this); // 자신 할당

            if (GetVariableValue(BTVars.TurnChannel, out BlackboardVariable<TurnChannel> targetChannel))
                TurnChannel = targetChannel.Value;
        }

        public void InvokeTurnEnd()
            => OnTurnEnd?.Invoke();

        public void SetVariableValue<T>(string variableName, T value)
        {
            Debug.Assert(!string.IsNullOrEmpty(variableName), $"Variable name is empty");

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