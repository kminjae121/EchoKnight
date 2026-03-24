using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem
{
    public class Unit : MonoBehaviour, ITurnable, IPoolable
    {
        [field: Header("Settings")]
        [field: SerializeField]
        public UnitSO unitSO { get; private set; }
        
        [field: SerializeField] public float TurnGauge { get; set; }
        
        [Header("Status")] public bool isMyTurn { get; private set; }
        public bool IsPlayerUnit { get; private set; }
        public float TurnSpeed { get; private set; }
        public Sprite UnitImage { get; private set; }
        public bool IsReadyDoAct => TurnGauge >= 100f;
        public string UnitName => unitSO != null ? unitSO.UnitName : "Unknown";
        
        [Header("Components")] protected Dictionary<Type, IUnitComponent> _components;
        public UnitManageRangeCompo RangesCompo { get; set; }
        public UnitAnimation AnimationCompo { get; private set; }
        
        [Header("Events")] public Action OnDeathEvent;
        public Action OnHitEvent;
        public UnityEvent OnStartTurnEvent;
        public UnityEvent OnEndTurnEvent;
        public float AddDefensivePower { get; set; }
        
        [field: SerializeField] public PoolingItemSO PoolingType { get; private set; }
        public GameObject GameObject => gameObject;
        
        private Pool _myPool;
        
        
        protected virtual void Awake()
        {
            AddUnitComponents();
            InitComponents();
            AfterInitComponents();
        }
        
        protected virtual void OnEnable()
        {
            InitializeData();
            RegisterEvents();
        }
        
        protected virtual void OnDisable()
        {
        }
        
        protected virtual void OnDestroy()
        {
            UnregisterEvents();
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
            if (AddDefensivePower != 0)
            {
                unitSO.DefensivePower -= AddDefensivePower;
            }
        
            AddDefensivePower = 0;
        
            OnStartTurnEvent?.Invoke();
        }
        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }
        
        public void ResetItem()
        {
            
        }
        
        public virtual void OnTurnEnd()
        {
            isMyTurn = false;
            OnEndTurnEvent?.Invoke();
        }
        
        protected virtual void Hit()
        {
        }
        
        protected virtual void Dead()
        {
            Bus<UnitDeadEvent>.Raise(new UnitDeadEvent(this));
        }
        
        private void AddUnitComponents()
        {
            _components = GetComponentsInChildren<IUnitComponent>()
                .ToDictionary(compo => compo.GetType());
        
            RangesCompo = GetUnitCompo<UnitManageRangeCompo>();
            AnimationCompo = GetUnitCompo<UnitAnimation>();
        }
        
        protected virtual void InitComponents()
        {
            foreach (var component in _components.Values)
                component.Initialize(this);
        }
        
        protected virtual void AfterInitComponents()
        {
            foreach (var component in _components.Values.OfType<IAfterInitialize>())
                component.AfterInitialize();
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