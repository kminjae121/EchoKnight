using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem
{
    public class Unit : MonoBehaviour, ITurnable
    {
        [field: Header("Settings")]
        [field: SerializeField] public UnitSO unitSO { get; private set; }
        [field: SerializeField] public float TurnGauge { get; set; }

        [Header("Status")]
        public bool isMyTurn { get; set; } = false;
        public bool IsPlayerUnit { get; set; }
        public float TurnSpeed { get; private set; }
        public Sprite UnitImage { get; private set; }
        public bool IsReadyDoAct => TurnGauge >= 100f;
        public string UnitName => unitSO != null ? unitSO.UnitName : "Unknown";

        [Header("Components")]
        protected readonly Dictionary<Type, IUnitComponent> _components = new();
        public UnitManageRangeCompo RangesCompo { get; set; }
        public UnitAnimation AnimationCompo { get; private set; }

        [Header("Events")]
        public Action OnDeathEvent { get; set; }
        public Action OnHitEvent;
        public UnityEvent OnStartTurnEvent;
        public UnityEvent OnEndTurnEvent;

        protected virtual void OnEnable()
        {
            InitializeData();
            AddUnitComponents();
            InitializeUnitComponents();
            AfterInitializeComponents();
            
            RegisterEvents();
        }
        
        protected virtual void OnDisable()
        {
            UnregisterEvents();
        }

        protected virtual void OnDestroy()
        {

        }

        private void InitializeData()
        {
            if (unitSO != null)
            {
                TurnSpeed = unitSO.turnSpeed;
                IsPlayerUnit = unitSO.isPlayerUnit;
                UnitImage = unitSO.UnitImage;
            }
            TurnGauge = 0f;
        }

        private void RegisterEvents()
        {
            OnHitEvent -= Hit;
            OnDeathEvent -= Dead;
            
            OnHitEvent += Hit;
            OnDeathEvent += Dead;
        }

        private void UnregisterEvents()
        {
            OnHitEvent -= Hit;
            OnDeathEvent -= Dead;
        }

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

        protected virtual void Hit() { }

        protected virtual void Dead()
        {
            Bus<UnitDeadEvent>.Raise(new UnitDeadEvent(this));
        }

        private void AddUnitComponents()
        {
            _components.Clear();
            var components = GetComponentsInChildren<IUnitComponent>();
            foreach (var component in components)
            {
                if (!_components.ContainsKey(component.GetType()))
                {
                    _components.Add(component.GetType(), component);
                }
            }
            
            RangesCompo = GetUnitCompo<UnitManageRangeCompo>();
            AnimationCompo = GetUnitCompo<UnitAnimation>();
        }

        private void InitializeUnitComponents()
        {
            foreach (var component in _components.Values)
            {
                component.Initialize(this);
            }
        }

        private void AfterInitializeComponents()
        {
            _components.Values.OfType<IAfterInitialize>()
                .ToList().ForEach(component => component.AfterInitialize());
        }

        public T GetUnitCompo<T>() where T : class, IUnitComponent 
        {
            return _components.GetValueOrDefault(typeof(T)) as T;
        }

        public IUnitComponent GetUnitCompo(Type type)
        {
            return _components.GetValueOrDefault(type);
        }
    }
}