using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using GameEventChannel;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem
{
    public class Unit : MonoBehaviour, ITurnable
    {
        [field: SerializeField] public UnitSO unitSO { get; private set; }

        public float TurnSpeed { get; private set; }

        public Sprite UnitImage { get; private set; }

        public UnityEvent OnStartTurnEvent;
        public UnityEvent OnEndTurnEvent;
        public UnityEvent OnHitEvent;
        
        
        public virtual void OnTurnStart()
        {
            isMyTurn = true;
            OnStartTurnEvent?.Invoke();
        }

        public virtual void OnTurnEnd()
        {
            isMyTurn = false;
            OnEndTurnEvent?.Invoke();
        }

        [field: SerializeField] public float TurnGauge { get; set; }

        public bool isMyTurn { get; set; } = false;
        
        public bool IsReadyDoAct => TurnGauge >= 100f;

        public string UnitName => unitSO.UnitName;
        public bool IsPlayerUnit { get; set; }
        
        public Action OnDeathEvent { get; private set; }

        protected readonly Dictionary<Type,IUnitComponent> _components = new();

        protected virtual void OnEnable()
        {
            AddUnitComponents();
            InitializeUnitComponents();
            AfterInitializeComponents();
            
            TurnSpeed = unitSO.turnSpeed;
            IsPlayerUnit = unitSO.isPlayerUnit;
            TurnGauge = 0f;
        }
        

        protected virtual void Dead()
        {
            Bus<UnitDeadEvent>.Raise(new UnitDeadEvent(this));
        }
        
        private void InitializeUnitComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        private void AfterInitializeComponents()
        {
            _components.Values.OfType<IAfterInitialize>()
                .ToList().ForEach(component => component.AfterInitialize());
        }

        private void AddUnitComponents()
        {
            GetComponentsInChildren<IUnitComponent>().ToList()
                .ForEach(component => _components.Add(component.GetType(), component));
        }
        
        public T GetUnitCompo<T>() where T : IUnitComponent => 
            (T)_components.GetValueOrDefault(typeof(T));

        public IUnitComponent GetUnitCompo(Type type)
            => _components.GetValueOrDefault(type);
    }
}