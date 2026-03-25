using Code.Core.Debugs;
using Code.UnitSystem.Enemies.AI;
using Code.UnitSystem.UnitComponent;
using Unity.Behavior;
using UnityEngine;

namespace Code.UnitSystem.Enemies
{
    public abstract class AbstractEnemyUnit : Unit
    {
        public BehaviorGraphAgent BTAgent { get; private set; }
        public PathMover PathMover { get; private set; }
        public TestAttackCompo AttackCompo { get; private set; }
        public TurnChannel TurnChannel { get; private set; }
        public UnitAnimation UnitAnimator { get; private set; }

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
            AttackCompo = GetUnitCompo<TestAttackCompo>();
            UnitAnimator = GetUnitCompo<UnitAnimation>();
        }

        protected virtual void Start()
        {
            //SetVariableValue(BTVars.Enemy, this);
            SetVariableValue(BTVars.UnitAnimator, UnitAnimator);

            if (GetVariableValue(BTVars.TurnChannel, out BlackboardVariable<TurnChannel> targetChannel))
                TurnChannel = targetChannel.Value;
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
            => true;

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